using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StylusEditorClassifier
{
  

    #region Format definition: Light Theme
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Default; //Colors.Brown;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Keyword; //Colors.OrangeRed;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Keyword2; //Colors.MediumSlateBlue;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Content; //Colors.Blue;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Function; //Colors.Teal;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Comment; //Colors.Green;
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
            this.ForegroundColor = Constants.Themes[VsTheme.Light].Comment; //Colors.Green;
        }
    }
    #endregion //Format definition: Light Theme

    #region Format definition: Dark Theme
    /// <summary>
    /// Defines an editor format for the StylusEditorClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.DefaultClassType + Constants.DarkSuffix)]
    [Name("StylusEditorClassifier_Dark")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StylusEditorClassifierFormat_Dark : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StylusEditorClassifier" classification type
        /// </summary>
        public StylusEditorClassifierFormat_Dark()
        {
            this.DisplayName = "StylusEditorClassifier_Dark"; //human readable version of the name
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Default; //Colors.Brown;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.KeywordClassType + Constants.DarkSuffix)]
    [Name("Stylus.KeywordClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class KeywordClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public KeywordClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Keyword Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Keyword; //Colors.OrangeRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.Keyword2ClassType + Constants.DarkSuffix)]
    [Name("Stylus.Keyword2ClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class Keyword2ClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public Keyword2ClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Keyword 2 Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Keyword2; //Colors.MediumSlateBlue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ContentClassType + Constants.DarkSuffix)]
    [Name("Stylus.ContentClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class ContentClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public ContentClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Content Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Content; //Colors.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.FunctionClassType + Constants.DarkSuffix)]
    [Name("Stylus.FunctionClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FunctionClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public FunctionClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Function Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Function; //Colors.Teal;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SingleLineCommentClassType + Constants.DarkSuffix)]
    [Name("Stylus.CommentClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public CommentClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Comment Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Comment; //Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.MultiLineCommentClassType + Constants.DarkSuffix)]
    [Name("Stylus.MultiCommentClassificationFormat_Dark")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class MultiLineCommentClassificationFormat_Dark : ClassificationFormatDefinition
    {
        public MultiLineCommentClassificationFormat_Dark()
        {
            this.DisplayName = "Stylus Multiline Comment Dark";
            this.ForegroundColor = Constants.Themes[VsTheme.Dark].Comment; //Colors.Green;
        }
    }
    #endregion //Format definition: Dark theme
}
