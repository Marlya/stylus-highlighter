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
            Boolean isComment = false;

            Boolean afterKeyword = false;
            foreach (var originals_str in parts)
            {
                this.GetClassificationSpan(originals_str, 1, ref index, ref isComment,
                    ref afterKeyword, span.Snapshot, ref classifications);

            }
            return classifications;
        }

        private Boolean GetClassificationSpan(String spanText, Int32 spaceSize, ref Int32 startIndex,
            ref Boolean isComment,
            ref Boolean afterKeyword,
            ITextSnapshot snapshot, ref List<ClassificationSpan> spans)
        {
            ClassificationSpan span = null;

            var str = spanText;
            
            if (String.IsNullOrWhiteSpace(str))
            {
                startIndex = startIndex + spaceSize;
                return false;
            }

            if (!isComment && !isMultiComment && !str.EndsWith(":") && str.IndexOf(":") > 0)
            {
                var start = str.IndexOf(":");
                var res1 = this.GetClassificationSpan(str.Substring(0, start+1), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                var res2 = this.GetClassificationSpan(str.Substring(start+1), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                return res1 || res2;
            }

            if (!isComment && !isMultiComment && str.Length > 1 && str.IndexOf("(") >= 0)
            {
                var start = str.IndexOf("(");
                Boolean res1 =false;
                if (start > 0)
                {
                    res1 = this.GetClassificationSpan(str.Substring(0, start), 0, ref startIndex,
                        ref isComment, ref afterKeyword, snapshot, ref spans);
                }
                var res2 = this.GetClassificationSpan("(", 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                Boolean res3 = false;
                if (!str.EndsWith("("))
                {
                    res3 = this.GetClassificationSpan(str.Substring(start + 1), 0, ref startIndex,
                        ref isComment, ref afterKeyword, snapshot, ref spans);
                }
                return res1 || res2 || res3;
            }

            if (!isComment && !isMultiComment && str.Length > 1 && str.IndexOf(")") >= 0)
            {
                var start = str.IndexOf(")");
                Boolean res1 = false;
                if (start > 0)
                {
                    res1 = this.GetClassificationSpan(str.Substring(0, start), 0, ref startIndex,
                        ref isComment, ref afterKeyword, snapshot, ref spans);
                }
                var res2 = this.GetClassificationSpan(")", 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                Boolean res3 = false;
                if (!str.EndsWith(")"))
                {
                    res3 = this.GetClassificationSpan(str.Substring(start + 1), 0, ref startIndex,
                        ref isComment, ref afterKeyword, snapshot, ref spans);
                }
                return res1 || res2 || res3;
            }

            if (!isComment && !isMultiComment && str.Trim().IndexOf("//") > 0)
            {
                var comment_start = str.IndexOf("//");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_start), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                var res2 = this.GetClassificationSpan(str.Substring(comment_start), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                return res1 || res2;
                //comment_start = str.IndexOf("//");
                //if (comment_start > 0)
                //{
                //    str = str.Substring(0, comment_start);
                //}
            }

            if (!isComment && !isMultiComment && str.IndexOf("/*") > 0 && !str.StartsWith("'"))
            {
                var comment_start = str.IndexOf("/*");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_start), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                var res2 = this.GetClassificationSpan(str.Substring(comment_start), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                return res1 || res2;
                //comment_start = str.IndexOf("/*");
                //if (comment_start > 0)
                //{
                //    str = str.Substring(0, comment_start);
                //}
            }

            if (isMultiComment && str.Length > 2 && !str.EndsWith("*/") && str.Contains("*/"))
            {
                var comment_end = str.IndexOf("*/");
                var res1 = this.GetClassificationSpan(str.Substring(0, comment_end+2), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                var res2 = this.GetClassificationSpan(str.Substring(comment_end+2), 0, ref startIndex,
                    ref isComment, ref afterKeyword, snapshot, ref spans);
                return res1 || res2;
                //if (comment_end >= 0)
                //{
                //    str = str.Substring(0, comment_end + 2);
                //}
            }

            //if (comment_end >= 0)
            //{
            //    span = new ClassificationSpan(new SnapshotSpan(snapshot, new Span(startIndex, str.Length)),
            //            _registry.GetClassificationType(Constants.MultiLineCommentClassType));

            //    startIndex += str.Length;
            //    isMultiComment = false;
            //    str = spanText.Substring(comment_end + 2);
            //    spans.Add(span);
            //}

            IClassificationType classificationType = null;
            
            #region detect Classification Type
            if (!afterKeyword && !isComment && !isMultiComment && (str.Equals("(") || str.Equals(")")))
            {
                classificationType = _registry.GetClassificationType(Constants.DefaultClassType);
            }
            else if (!afterKeyword && !isComment && !isMultiComment && Constants.Keywords2.Contains(str.Trim().Trim('(').Trim('(').Trim(':')))
            {
                classificationType = _registry.GetClassificationType(Constants.Keyword2ClassType);
            }
            else if (!afterKeyword && !isComment && !isMultiComment && Constants.Keywords.Contains(str.Trim().Trim('(').Trim('(').Trim(':')))
            {
                classificationType = _registry.GetClassificationType(Constants.KeywordClassType);
                afterKeyword = !str.Contains("//n");
            }
            else if (!isMultiComment && (str.Trim().StartsWith("//") || isComment))
            {
                classificationType =  _registry.GetClassificationType(Constants.SingleLineCommentClassType);
                isComment = !str.Contains("//n");
                afterKeyword = false;
            }
            else if (!isComment && (isMultiComment || str.StartsWith("/*")))
            {
                classificationType =   _registry.GetClassificationType(Constants.MultiLineCommentClassType);
                isMultiComment = !str.Contains("*/");
            }
            else if (afterKeyword)
            {
                classificationType = _registry.GetClassificationType(Constants.ContentClassType);
            }
            else
            {
                classificationType =  _registry.GetClassificationType(Constants.DefaultClassType);
            }
            #endregion

            span =
                     new ClassificationSpan(new SnapshotSpan(snapshot, new Span(startIndex, str.Length)),
                         classificationType);

            startIndex += str.Length + spaceSize;
            
            spans.Add(span);

            //if (comment_start > 0)
            //{
            //    str = spanText.Substring(comment_start);
            //    span =
            //         new ClassificationSpan(new SnapshotSpan(snapshot, new Span(startIndex, str.Length)),
            //             _registry.GetClassificationType(Constants.SingleLineCommentClassType));
            //    startIndex += str.Length;
            //    isComment = true;
            //    afterKeyword = false;
            //    spans.Add(span);
            //}

            //if (isComment && str.Contains("//n"))
            //{
            //    isComment = false;
            //}

            //if (afterKeyword && str.Contains("//n"))
            //{
            //    afterKeyword = false;
            //}

            return true;
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
