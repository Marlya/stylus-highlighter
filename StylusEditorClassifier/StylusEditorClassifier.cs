using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StylusEditorClassifier
{

    #region Provider definition
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(Constants.ContentType)]
    internal class StylusEditorClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty
                (delegate { return new StylusEditorClassifier(buffer, ClassificationRegistry); });
        }
    }
    #endregion //provider def

    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    class StylusEditorClassifier : IClassifier
    {
        IClassificationType _classificationType;
        private IClassificationTypeRegistryService _registry;
        private ITextBuffer _buffer;
        
        State currentState = State.None;

        internal StylusEditorClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            this._registry = registry;
            this._buffer = buffer;

            this._buffer.Changed += BufferChanged;
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            foreach (var change in e.Changes)
            {
                var startMC = this.GetMultiCommentStartIndeces();
                var endMC = this.GetMultiCommentEndIndeces();

                if (change.NewText == Environment.NewLine)
                {
                    this.MoveMultiComment(startMC, endMC, change.OldEnd, change.Delta);
                    return;
                }

                var update = -1;
                var newText = change.NewText;//e.After.GetLineFromPosition(change.NewPosition).GetText();
                var oldText = change.OldText;//e.Before.GetLineFromPosition(change.OldPosition).GetText();

                if (startMC != null && oldText.Contains("/*") && !newText.Contains("/*"))
                {
                    var ind = oldText.IndexOf("/*");
                    if (ind >= 0)
                    {
                        var position = e.Before.GetLineFromPosition(change.OldPosition).Start + ind;
                        startMC.Remove(position);
                    }

                    if (endMC == null || !endMC.Any(ep => ep >= change.OldPosition))
                        update = Int32.MinValue;
                    else
                        update = endMC.Where(ep => ep >= change.OldPosition).OrderBy(ep => ep).First();

                    this.MoveMultiComment(startMC, endMC, change.OldEnd, change.Delta);
                }

                if (!oldText.Contains("/*") && newText.Contains("/*"))
                {
                    if (endMC == null || !endMC.Any(ep => ep >= change.OldPosition))
                        update = Int32.MinValue;
                    else
                        update = endMC.Where(ep => ep >= change.OldPosition).OrderBy(ep => ep).First();

                    this.MoveMultiComment(startMC, endMC, change.OldEnd, change.Delta);

                    var position = e.After.GetLineFromPosition(change.NewPosition).Start + newText.IndexOf("/*");
                    if (startMC == null)
                    {
                        startMC = new List<Int32>();
                    }
                    if (!startMC.Contains(position))
                    {
                        startMC.Add(position);
                        this.SetMultiCommentStartIndeces(startMC);
                    }
                }


                if (endMC != null && oldText.Contains("*/") && !newText.Contains("*/"))
                {
                    var ind = oldText.IndexOf("*/");
                    if (ind >= 0)
                    {
                        var position = e.Before.GetLineFromPosition(change.OldPosition).Start + ind + 2;
                        endMC.Remove(position);
                    }

                    if (!endMC.Any(ep => ep >= change.OldPosition))
                        update = Int32.MinValue;
                    else
                        update = endMC.Where(ep => ep >= change.OldPosition).OrderBy(ep => ep).First();

                    this.MoveMultiComment(startMC, endMC, change.OldEnd, change.Delta);
                }

                if (!oldText.Contains("*/") && newText.Contains("*/"))
                {
                    if (endMC == null)
                    {
                        endMC = new List<Int32>();
                    }

                    var position = e.After.GetLineFromPosition(change.NewPosition).Start + newText.IndexOf("*/") + 2;

                    if (!endMC.Any(ep => ep >= change.OldPosition))
                        update = Int32.MinValue;
                    else
                        update = endMC.Where(ep => ep >= change.OldPosition + 2).OrderBy(ep => ep).First();

                    this.MoveMultiComment(startMC, endMC, change.OldEnd, change.Delta);

                    if (!endMC.Contains(position))
                    {
                        endMC.Add(position);
                        this.SetMultiCommentEndIndeces(endMC);
                    }

                }

                if (update != -1)
                {
                    SnapshotSpan paragraph = update == Int32.MinValue
                        ? new SnapshotSpan(e.After, change.NewEnd, e.After.Length - change.NewEnd)
                        : new SnapshotSpan(e.After, change.NewEnd, update - change.NewEnd);

                    var temp = this.ClassificationChanged;
                    if (temp != null)
                        temp(this, new ClassificationChangedEventArgs(paragraph));
                }
                else
                {
                    if (this.DetetectState(change.OldPosition) == State.IsMultiComment)
                    {
                        this.MoveMultiComment(startMC, endMC, change.OldPosition, change.Delta);
                    }
                }
            }
        }

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="span">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (span.Start == 0)
            {
                currentState = State.None;
            }
            currentState = this.DetetectState(span.Start);
            
            //create a list to hold the results
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            String text = _buffer.CurrentSnapshot.GetText(span);

            if (this.CheckForFunction(text.Trim()))
            {
                Int32 startIndex = span.Start;

                classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(startIndex, text.IndexOf("("))),
                          _registry.GetClassificationType(Constants.FunctionClassType)));

                String[] parts = text.Substring(text.IndexOf("(")).Split(' ');
                Int32 index = span.Start + text.IndexOf("(");

                foreach (var originals_str in parts)
                {
                    this.GetClassificationSpan(originals_str,
                        parts.Length == 1 && originals_str == parts[parts.Length - 1] ? 0 : 1, ref index,
                        span.Snapshot, ref classifications);

                }
            }
            else
            {
                String[] parts = text.Split(' ');
                Int32 index = span.Start;

                foreach (var originals_str in parts)
                {
                    this.GetClassificationSpan(originals_str,
                        parts.Length == 1 && originals_str == parts[parts.Length - 1] ? 0 : 1, ref index,
                        span.Snapshot, ref classifications);

                }
            }

            if (span.End == span.Snapshot.Length)
            {
                currentState = State.None;
            }
            return classifications;
        }

        private State DetetectState(Int32 index)
        {
            var startMultiComments = this.GetMultiCommentStartIndeces();
            var endMultiComments = this.GetMultiCommentEndIndeces();

            if (startMultiComments !=null && startMultiComments.Any(st => st < index))
            {
                Int32 start = startMultiComments.Where(st => st < index).Min(st => index- st);
                start = index - start;

                if(endMultiComments == null || !endMultiComments.Any(ind=> ind > start))
                    return State.IsMultiComment;

                var ends = endMultiComments.Where(st => st > start).ToList();
                Int32 end =ends.Min(st => st - start);
                end = end + start;

                if(end > index)
                    return State.IsMultiComment;

            }
            return currentState == State.None || currentState == State.IsMultiComment ? State.Default : currentState;
        }
        #region Multi comment

        private List<Int32> GetMultiCommentStartIndeces()
        {
            List<Int32> startIndeces = null;
            if (this._buffer.Properties.ContainsProperty("StartMultiComment"))
            {
                startIndeces = this._buffer.Properties.GetProperty<List<Int32>>("StartMultiComment");
            }
            return startIndeces;
        }

        private void SetMultiCommentStartIndeces(List<Int32> indeces)
        {
            if (this._buffer.Properties.ContainsProperty("StartMultiComment"))
            {
               this._buffer.Properties.RemoveProperty("StartMultiComment");
            }
            this._buffer.Properties.AddProperty("StartMultiComment", indeces);
        }

        private List<Int32> GetMultiCommentEndIndeces()
        {
            List<Int32> startIndeces = null;
            if (this._buffer.Properties.ContainsProperty("EndMultiComment"))
            {
                startIndeces = this._buffer.Properties.GetProperty<List<Int32>>("EndMultiComment");
            }
            return startIndeces;
        }

        private void SetMultiCommentEndIndeces(List<Int32> indeces)
        {
            if (this._buffer.Properties.ContainsProperty("EndMultiComment"))
            {
                this._buffer.Properties.RemoveProperty("EndMultiComment");
            }
            this._buffer.Properties.AddProperty("EndMultiComment", indeces);
        }

        private void AddMultiCommentStart(Int32 index)
        {
            List<Int32> startIndeces = this.GetMultiCommentStartIndeces();

            if(startIndeces == null)
                startIndeces = new List<Int32>();

            if (!startIndeces.Contains(index))
            {
                startIndeces.Add(index);
                this.SetMultiCommentStartIndeces(startIndeces);
            }


        }

        private void AddMultiCommentEnd(Int32 index)
        {
            List<Int32> endIndeces = this.GetMultiCommentEndIndeces();

            if (endIndeces == null)
                endIndeces = new List<Int32>();

            if (!endIndeces.Contains(index+2))
            {
                endIndeces.Add(index+2);
                this.SetMultiCommentEndIndeces(endIndeces);
            }
        }

        private void MoveMultiComment(List<Int32> startIndeces, List<Int32> endIndeces, Int32 startWithPosition, Int32 delta)
        {
            if (startIndeces != null)
            {
                var startToMove = startIndeces.Where(st => st >= startWithPosition).ToList();
                startIndeces.RemoveAll(startToMove.Contains);
                startIndeces.AddRange(startToMove.Where(st => !startIndeces.Contains(st)).Select(st => st + delta));
                this.SetMultiCommentStartIndeces(startIndeces);
            }

            if (endIndeces != null)
            {
                var endToMove = endIndeces.Where(st => st > startWithPosition).ToList();
                endIndeces.RemoveAll(endToMove.Contains);
                endIndeces.AddRange(endToMove.Where(st => !endIndeces.Contains(st)).Select(st => st + delta));
                this.SetMultiCommentEndIndeces(endIndeces);
            }
        }

       
        #endregion
        private Boolean GetClassificationSpan(String spanText, Int32 spaceSize, ref Int32 startIndex,
            ITextSnapshot snapshot, ref List<ClassificationSpan> spans)
        {
            ClassificationSpan span = null;

            var str = spanText;
            
            if (String.IsNullOrWhiteSpace(str))
            {
                startIndex = startIndex + str.Length + spaceSize;
                if (str == Environment.NewLine && 
                    (currentState == State.AfterKeyword || currentState == State.AfterKeywordAfterBracket || currentState == State.IsComment))
                    currentState = State.Default;
                return false;
            }

            Boolean res = this.CheckForSpecialSymbols(str, spaceSize, ref startIndex, snapshot, ref spans);

            if(res) 
                return true;

            IClassificationType classificationType = null;
            
            #region detect Classification Type

            if ((currentState != State.IsComment && currentState != State.IsMultiComment) &&
                (str.Equals("(") || str.Equals(")")))
            {
                classificationType = _registry.GetClassificationType(Constants.DefaultClassType);

                if (str.Equals("("))
                {
                    if (currentState == State.Default)
                        currentState = State.Bracket;

                    //if (currentState == State.AfterKeyword)
                    //    currentState = State.BracketAfterKeyword;
                }

                if (str.Equals(")"))
                {
                    if (currentState == State.Bracket || currentState == State.AfterKeywordAfterBracket )
                        currentState = State.Default;
                }
            }
            else if (currentState == State.Default &&
                     Constants.Keywords2.Contains(str.Trim().Trim('(').Trim('(').Trim(':')))
            {
                classificationType = _registry.GetClassificationType(Constants.Keyword2ClassType);
            }
            else if ((currentState == State.Default || currentState == State.Bracket)
                     &&
                     (
                         Constants.CssKeys.Contains(str.Trim().Trim(':'))
                         ||
                         (str.Trim().StartsWith("-webkit-") &&
                          Constants.CssKeys.Contains(str.Trim().Trim(':').Replace("-webkit-", "")))
                         ||
                         (str.Trim().StartsWith("-moz-") &&
                          Constants.CssKeys.Contains(str.Trim().Trim(':').Replace("-moz-", "")))
                         ||
                         (str.Trim().StartsWith("-o-") &&
                          Constants.CssKeys.Contains(str.Trim().Trim(':').Replace("-o-", "")))
                         ||
                         (str.Trim().StartsWith("-ms-") &&
                          Constants.CssKeys.Contains(str.Trim().Trim(':').Replace("-ms-", "")))
                         )
                )
            {
                classificationType = _registry.GetClassificationType(Constants.KeywordClassType);
                currentState = !str.Contains("\n")
                    ? (currentState == State.Bracket ? State.AfterKeywordAfterBracket : State.AfterKeyword)
                    : State.Default;
            }
            else if (currentState != State.IsMultiComment &&
                     (str.Trim().StartsWith("//") || currentState == State.IsComment))
            {
                classificationType = _registry.GetClassificationType(Constants.SingleLineCommentClassType);
                currentState = !str.Contains("\n") ? State.IsComment : State.Default;
            }
            else if (currentState != State.IsComment &&
                     (currentState == State.IsMultiComment || str.StartsWith("/*")))
            {
                classificationType = _registry.GetClassificationType(Constants.MultiLineCommentClassType);
                currentState = !str.Contains("*/") ? State.IsMultiComment : State.Default;
                if (str.StartsWith("/*"))
                {
                    this.AddMultiCommentStart(startIndex);
                }
                if (str.Contains("*/"))
                {
                    this.AddMultiCommentEnd(startIndex + str.IndexOf("*/"));
                }

            }
            else if (currentState == State.AfterKeyword || currentState == State.AfterKeywordAfterBracket)
            {
                classificationType = _registry.GetClassificationType(Constants.ContentClassType);
                currentState = !str.Contains("\n") ? currentState : State.Default;
            }
            else
            {
                classificationType = _registry.GetClassificationType(Constants.DefaultClassType);
            }

            #endregion

            

            span = new ClassificationSpan(new SnapshotSpan(snapshot, new Span(startIndex, str.Length)),
                         classificationType);

            startIndex += str.Length + spaceSize;
            
            spans.Add(span);
            return true;
        }

        private Boolean ContainsSpecilaSymbolInString(SpecialSymbol symbol, String str)
        {
            var index = str.IndexOf(symbol.Symbol);
            if (index < 0) 
                return false;
            if (symbol.ValidStates != null && symbol.ValidStates.All(state => state != currentState))
                return false;
            if (symbol.NotValidStates != null && symbol.NotValidStates.Any(state => state == currentState))
                return false;
            if (!symbol.StartsWithZero && index == 0)
                return false;
            if (symbol.Include == IncludeType.IncludeToLeft && index == str.Length - symbol.Symbol.Length)
                return false;
            if (symbol.Include == IncludeType.Exclude && index == 0 && str.Length == symbol.Symbol.Length)
                return false;
            if (symbol.UnsuitableStringBeginnigns != null && symbol.UnsuitableStringBeginnigns.Any(str.StartsWith))
                return false;
            return true;
        }

        private Boolean CheckForFunction(String spanText)
        {
            Regex funcReg = new Regex("^[a-zA-Z-]+[ ]*[(]");
            return funcReg.IsMatch(spanText);
        }

        private Boolean CheckForSpecialSymbols(String spanText, Int32 move, ref Int32 startIndex,
            ITextSnapshot snapshot, ref List<ClassificationSpan> spans)
        {
            var str = spanText;

            SpecialSymbol symbol = Constants.SpecialSymbols.FirstOrDefault
                (smbl => this.ContainsSpecilaSymbolInString(smbl, str));

            if (symbol == null)
            {
                return false;
            }

            var index = str.IndexOf(symbol.Symbol);

            Boolean res1 = false, res2 = false, res3 = false;

            //left part
            if (symbol.Include == IncludeType.IncludeToLeft)
            {
                res1 = this.GetClassificationSpan(str.Substring(0, index + symbol.Symbol.Length), 0, ref startIndex,
                    snapshot, ref spans);
            }
            else if (symbol.Include == IncludeType.IncludeToRight
                     || (symbol.Include == IncludeType.Exclude && index > 0))
            {
                res1 = this.GetClassificationSpan(str.Substring(0, index), 0, ref startIndex,
                    snapshot, ref spans);
            }
            //middle
            if (symbol.Include == IncludeType.Exclude)
            {
                res2 = this.GetClassificationSpan(symbol.Symbol, 0, ref startIndex,
                    snapshot, ref spans);
            }
            //right part
            if (symbol.Include == IncludeType.IncludeToLeft
                || (symbol.Include == IncludeType.Exclude && index != str.Length - symbol.Symbol.Length))
            {
                res3 = this.GetClassificationSpan(str.Substring(index + symbol.Symbol.Length), 0, ref startIndex,
                    snapshot, ref spans);
            }
            else if (symbol.Include == IncludeType.IncludeToRight)
            {
                res3 = this.GetClassificationSpan(str.Substring(index), 0, ref startIndex,
                    snapshot, ref spans);
            }
            startIndex = startIndex + move;
            return res1 || res2 || res3;
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
    #endregion //Classifier
}
