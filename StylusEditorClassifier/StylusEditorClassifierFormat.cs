using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StylusEditorClassifier
{
    #region Format definition
    /// <summary>
    /// Defines an editor format for the StylusEditorClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.DefaultClassType)]
    [Name("StylusEditorClassifier")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StylusEditorClassifierFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StylusEditorClassifier" classification type
        /// </summary>
        public StylusEditorClassifierFormat()
        {
            this.DisplayName = "StylusEditorClassifier"; //human readable version of the name
            this.ForegroundColor = Colors.Brown;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.KeywordClassType)]
    [Name("Stylus.KeywordClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class KeywordClassificationFormat : ClassificationFormatDefinition
    {
        public KeywordClassificationFormat()
        {
            this.DisplayName = "Stylus Keyword";
            this.ForegroundColor = Colors.OrangeRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Keyword2ClassType)]
    [Name("Stylus.Keyword2ClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword2ClassificationFormat : ClassificationFormatDefinition
    {
        public Keyword2ClassificationFormat()
        {
            this.DisplayName = "Stylus Keyword 2";
            this.ForegroundColor = Colors.MediumSlateBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ContentClassType)]
    [Name("Stylus.ContentClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ContentClassificationFormat : ClassificationFormatDefinition
    {
        public ContentClassificationFormat()
        {
            this.DisplayName = "Stylus Content";
            this.ForegroundColor = Colors.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.FunctionClassType)]
    [Name("Stylus.FunctionClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FunctionClassificationFormat : ClassificationFormatDefinition
    {
        public FunctionClassificationFormat()
        {
            this.DisplayName = "Stylus Function";
            this.ForegroundColor = Colors.Teal;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SingleLineCommentClassType)]
    [Name("Stylus.CommentClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentClassificationFormat : ClassificationFormatDefinition
    {
        public CommentClassificationFormat()
        {
            this.DisplayName = "Stylus Comment";
            this.ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.MultiLineCommentClassType)]
    [Name("Stylus.MultiCommentClassificationFormat")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class MultiLineCommentClassificationFormat : ClassificationFormatDefinition
    {
        public MultiLineCommentClassificationFormat()
        {
            this.DisplayName = "Stylus Multiline Comment";
            this.ForegroundColor = Colors.Green;
        }
    }
    #endregion //Format definition
}
