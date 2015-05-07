using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StylusEditorClassifier
{
    internal static class StylusEditorClassifierClassificationDefinition
    {
        [Export]
        [Name(Constants.ContentType)]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition ContentTypeDefinition { get; set; }

        [Export]
        [FileExtension(Constants.FileExtension)]
        [ContentType(Constants.ContentType)]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition { get; set; }

        /// <summary>
        /// Defines the "StylusEditorClassifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.DefaultClassType)]
        internal static ClassificationTypeDefinition StylusEditorClassifierType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.KeywordClassType)]
        internal static ClassificationTypeDefinition StylusKeywordClassificationType { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.Keyword2ClassType)]
        internal static ClassificationTypeDefinition StylusKeyword2ClassificationType { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.ContentClassType)]
        internal static ClassificationTypeDefinition StylusContentClassificationType { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.SingleLineCommentClassType)]
        internal static ClassificationTypeDefinition StylusCommentClassificationType { get; set; }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Constants.MultiLineCommentClassType)]
        internal static ClassificationTypeDefinition StylusMultiLineCommentClassificationType { get; set; }
    }
}
