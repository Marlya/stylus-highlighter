using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection.Emit;
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

        internal StylusEditorClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            this._registry = registry;
            this._buffer = buffer;
            //_classificationType = registry.GetClassificationType("StylusEditorClassifier");
        }

        Boolean isMultiComment = false;

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="trackingSpan">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            //create a list to hold the results
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            String text = _buffer.CurrentSnapshot.GetText(span);

            String[] parts = text.Split(' ');
            Int32 index = span.Start;
            //Boolean isComment = false;

            //Boolean afterKeyword = false;
            State currentState = State.Default;

            foreach (var originals_str in parts)
            {
                this.GetClassificationSpan(originals_str, 1, ref index,
                    span.Snapshot, ref classifications, ref currentState);

            }
            return classifications;
        }

        private Boolean GetClassificationSpan(String spanText, Int32 spaceSize, ref Int32 startIndex,
            ITextSnapshot snapshot, ref List<ClassificationSpan> spans, ref State currentState)
        {
            ClassificationSpan span = null;

            var str = spanText;
            
            if (String.IsNullOrWhiteSpace(str))
            {
                startIndex = startIndex + spaceSize;
                return false;
            }

            Boolean res = this.CheckForSpecialSymbols(str, ref startIndex, snapshot, ref spans, ref currentState);

            if(res) return true;

            IClassificationType classificationType = null;
            
            #region detect Classification Type
            
            if (currentState == State.Default && (str.Equals("(") || str.Equals(")")))
            {
                classificationType = _registry.GetClassificationType(Constants.DefaultClassType);
            }
            else if (currentState == State.Default && Constants.Keywords2.Contains(str.Trim().Trim('(').Trim('(').Trim(':')))
            {
                classificationType = _registry.GetClassificationType(Constants.Keyword2ClassType);
            }
            else if (currentState == State.Default 
                && 
                    (
                        Constants.Keywords.Contains(str.Trim().Trim(':'))
                        || (str.Trim().StartsWith("-webkit-") && Constants.Keywords.Contains(str.Trim().Trim(':').Replace("-webkit-", "")))
                        || (str.Trim().StartsWith("-moz-") && Constants.Keywords.Contains(str.Trim().Trim(':').Replace("-moz-", "")))
                        || (str.Trim().StartsWith("-o-") && Constants.Keywords.Contains(str.Trim().Trim(':').Replace("-o-", "")))
                        || (str.Trim().StartsWith("-ms-") && Constants.Keywords.Contains(str.Trim().Trim(':').Replace("-ms-", "")))
                    )
                )
            {
                classificationType = _registry.GetClassificationType(Constants.KeywordClassType);
                currentState = !str.Contains("//n")?State.AfterKeyword : State.Default;
            }
            else if (!isMultiComment && (str.Trim().StartsWith("//") || currentState == State.IsComment))
            {
                classificationType =  _registry.GetClassificationType(Constants.SingleLineCommentClassType);
                currentState = State.IsComment;
            }
            else if (currentState != State.IsComment && (isMultiComment || str.StartsWith("/*")))
            {
                classificationType =   _registry.GetClassificationType(Constants.MultiLineCommentClassType);
                isMultiComment = !str.Contains("*/");
            }
            else if (currentState == State.AfterKeyword)
            {
                classificationType = _registry.GetClassificationType(Constants.ContentClassType);
            }
            else
            {
                classificationType =  _registry.GetClassificationType(Constants.DefaultClassType);
            }
            #endregion

            span = new ClassificationSpan(new SnapshotSpan(snapshot, new Span(startIndex, str.Length)),
                         classificationType);

            startIndex += str.Length + spaceSize;
            
            spans.Add(span);
            return true;
        }

        private Boolean CheckForSpecialSymbols(String spanText, ref Int32 startIndex,
            ITextSnapshot snapshot, ref List<ClassificationSpan> spans, ref State currentState)
        {
            var str = spanText;

            if (currentState!=State.IsComment && !isMultiComment && !str.EndsWith(":") && str.IndexOf(":") > 0)
            {
                var start = str.IndexOf(":");
                var res1 = this.GetClassificationSpan(str.Substring(0, start + 1), 0, ref startIndex,
                   snapshot, ref spans, ref currentState);
                var res2 = this.GetClassificationSpan(str.Substring(start + 1), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                return res1 || res2;
            }

            if (currentState != State.IsComment && !isMultiComment && str.Length > 1 && str.IndexOf("(") >= 0)
            {
                var start = str.IndexOf("(");
                Boolean res1 = false;
                if (start > 0)
                {
                    res1 = this.GetClassificationSpan(str.Substring(0, start), 0, ref startIndex,
                        snapshot, ref spans, ref currentState);
                }
                var res2 = this.GetClassificationSpan("(", 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                Boolean res3 = false;
                if (!str.EndsWith("("))
                {
                    res3 = this.GetClassificationSpan(str.Substring(start + 1), 0, ref startIndex,
                        snapshot, ref spans, ref currentState);
                }
                return res1 || res2 || res3;
            }

            if (currentState!=State.IsComment && !isMultiComment && str.Length > 1 && str.IndexOf(")") >= 0)
            {
                var start = str.IndexOf(")");
                Boolean res1 = false;
                if (start > 0)
                {
                    res1 = this.GetClassificationSpan(str.Substring(0, start), 0, ref startIndex,
                        snapshot, ref spans, ref currentState);
                }
                var res2 = this.GetClassificationSpan(")", 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                Boolean res3 = false;
                if (!str.EndsWith(")"))
                {
                    res3 = this.GetClassificationSpan(str.Substring(start + 1), 0, ref startIndex,
                        snapshot, ref spans, ref currentState);
                }
                return res1 || res2 || res3;
            }

            if (currentState!=State.IsComment && !isMultiComment && str.Trim().IndexOf("//") > 0)
            {
                var comment_start = str.IndexOf("//");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_start), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                var res2 = this.GetClassificationSpan(str.Substring(comment_start), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                return res1 || res2;
            }

            if (currentState != State.IsComment && !isMultiComment && str.IndexOf("/*") > 0 && !str.StartsWith("'"))
            {
                var comment_start = str.IndexOf("/*");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_start), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                var res2 = this.GetClassificationSpan(str.Substring(comment_start), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                return res1 || res2;
            }

            if (isMultiComment && str.Length > 2 && !str.EndsWith("*/") && str.Contains("*/"))
            {
                var comment_end = str.IndexOf("*/");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_end + 2), 0, ref startIndex,
                    snapshot, ref spans, ref currentState);
                var res2 = this.GetClassificationSpan(str.Substring(comment_end + 2), 0, ref startIndex,
                   snapshot, ref spans, ref currentState);
                return res1 || res2;
            }
            return false;
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
