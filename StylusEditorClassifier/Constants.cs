using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace StylusEditorClassifier
{
    internal class Constants
    {
        public const string ContentType = "stylus";
        public const string FileExtension = ".styl";
        public const string DefaultClassType = "StylusEditorClassifier";
        public const string KeywordClassType = "stylus.keyword";
        public const string Keyword2ClassType = "stylus.keyword2";
        public const string ContentClassType = "stylus.content";
        public const string FunctionClassType = "stylus.function";
        public const string SingleLineCommentClassType = "stylus.singlelineComment";
        public const string MultiLineCommentClassType = "stylus.multilineComment";

        public const string DarkSuffix = "_Dark";

        public static readonly IDictionary<VsTheme, ThemeColors> Themes = new Dictionary<VsTheme, ThemeColors>(3)
        {
            {VsTheme.Light, new ThemeColors()
            {
                Comment = Colors.Green,
                Content = Colors.Blue,
                Default = Colors.Brown,
                Function = Colors.Teal,
                Keyword = Colors.OrangeRed,
                Keyword2 = Colors.MediumSlateBlue
            }},
            {VsTheme.Dark, new ThemeColors(){
                Comment = Colors.Aquamarine,
                Content = Colors.SkyBlue,
                Default = Colors.Bisque,
                Function = Colors.Aqua,
                Keyword = Colors.LightSalmon,
                Keyword2 = Colors.Plum
            }},
            {VsTheme.Blue, new ThemeColors(){
                Comment = Colors.Green,
                Content = Colors.Blue,
                Default = Colors.Brown,
                Function = Colors.Teal,
                Keyword = Colors.OrangeRed,
                Keyword2 = Colors.MediumSlateBlue
            }}
            
        };


        //public static ThemeColors GetThemeColors()
        //{
        //    VsTheme currentTheme = ThemeUtil.GetCurrentTheme();
        //    if (Constants.Themes.ContainsKey(currentTheme))
        //    {
        //        return Constants.Themes[currentTheme];
        //    }
        //    else
        //    {
        //        return Constants.Themes[VsTheme.Blue];
        //    }
        //}

	    //public static readonly ThemeColors CurrentThemeColors = Themes[ThemeUtil.GetCurrentTheme()];

        public static readonly List<SpecialSymbol> SpecialSymbols = new List<SpecialSymbol>
        {
            new SpecialSymbol()
            {
                Symbol = "//", 
                Include = IncludeType.IncludeToRight, 
                StartsWithZero = false,
                NotValidStates = new List<State>{State.IsMultiComment, State.IsComment}
            },
            new SpecialSymbol()
            {
                Symbol = "/*",
                Include = IncludeType.IncludeToRight,
                StartsWithZero = false, 
                UnsuitableStringBeginnigns = new List<string>{"'","\""},
                NotValidStates = new List<State>{State.IsMultiComment, State.IsComment}
            },
            new SpecialSymbol()
            {
                Symbol = "*/",
                Include = IncludeType.IncludeToLeft,
                StartsWithZero = true, 
                ValidStates = new List<State>{State.IsMultiComment}
            },
            new SpecialSymbol()
            {
                Symbol = ":",
                Include = IncludeType.IncludeToLeft,
                StartsWithZero = false,
                NotValidStates = new List<State>{State.IsMultiComment, State.IsComment}
            },
            new SpecialSymbol()
            {
                Symbol = "(",
                Include = IncludeType.Exclude,
                StartsWithZero = true,
                NotValidStates = new List<State>{State.IsMultiComment, State.IsComment}
            },
            new SpecialSymbol()
            {
                Symbol = ")",
                Include = IncludeType.Exclude,
                StartsWithZero = true,
                NotValidStates = new List<State>{State.IsMultiComment, State.IsComment}
            },
        };

        public static readonly HashSet<String> CssKeys = new HashSet<String>
        {
            "accelerator",
            "animation",
            "azimuth",
            "background",
            "background-attachment",
            "background-color",
            "background-clip",
            "background-image",
            "background-position",
            "background-position-x",
            "background-position-y",
            "background-repeat",
            "background-size",
            "behavior",
            "border",
            "border-bottom",
            "border-bottom-color",
            "border-bottom-style",
            "border-bottom-width",
            "border-collapse",
            "border-color",
            "border-left",
            "border-left-color",
            "border-left-style",
            "border-left-width",
            "border-radius",
            "border-right",
            "border-right-color",
            "border-right-style",
            "border-right-width",
            "border-spacing",
            "border-style",
            "border-top",
            "border-top-color",
            "border-top-style",
            "border-top-width",
            "border-width",
            "bottom",
            "box-shadow",
            "caption-side",
            "clear",
            "clip",
            "color",
            "content",
            "counter-increment",
            "counter-reset",
            "cue",
            "cue-after",
            "cue-before",
            "cursor",
            "direction",
            "display",
            "elevation",
            "empty-cells",
            "filter",
            "flex",
            "float",
            "font",
            "font-family",
            "font-size",
            "font-size-adjust",
            "font-stretch",
            "font-style",
            "font-variant",
            "font-weight",
            "height",
            "ime-mode",
            "include-source",
            "layer-background-color",
            "layer-background-image",
            "layout-flow",
            "layout-grid",
            "layout-grid-char",
            "layout-grid-char-spacing",
            "layout-grid-line",
            "layout-grid-mode",
            "layout-grid-type",
            "left",
            "letter-spacing",
            "line-break",
            "line-height",
            "list-style",
            "list-style-image",
            "list-style-position",
            "list-style-type",
            "margin",
            "margin-bottom",
            "margin-left",
            "margin-right",
            "margin-top",
            "marker-offset",
            "marks",
            "max-height",
            "max-width",
            "min-height",
            "min-width",
            "opacity",
            "orphans",
            "outline",
            "outline-color",
            "outline-style",
            "outline-width",
            "overflow",
            "overflow-X",
            "overflow-Y",
            "overflow-x",
            "overflow-y",
            "padding",
            "padding-bottom",
            "padding-left",
            "padding-right",
            "padding-top",
            "page",
            "page-break-after",
            "page-break-before",
            "page-break-inside",
            "pause",
            "pause-after",
            "pause-before",
            "pitch",
            "pitch-range",
            "play-during",
            "position",
            "quotes",
            "-replace",
            "richness",
            "right",
            "ruby-align",
            "ruby-overhang",
            "ruby-position",
            "-set-link-source",
            "size",
            "speak",
            "speak-header",
            "speak-numeral",
            "speak-punctuation",
            "speech-rate",
            "stress",
            "scrollbar-arrow-color",
            "scrollbar-base-color",
            "scrollbar-dark-shadow-color",
            "scrollbar-face-color",
            "scrollbar-highlight-color",
            "scrollbar-shadow-color",
            "scrollbar-3d-light-color",
            "scrollbar-track-color",
            "table-layout",
            "text-align",
            "text-align-last",
            "text-decoration",
            "text-indent",
            "text-justify",
            "text-overflow",
            "text-shadow",
            "text-transform",
            "text-autospace",
            "text-kashida-space",
            "text-underline-position",
            "top",
            "transform",
            "unicode-bidi",
            "-use-link-source",
            "vertical-align",
            "visibility",
            "voice-family",
            "volume",
            "white-space",
            "widows",
            "width",
            "word-break",
            "word-spacing",
            "word-wrap",
            "writing-mode",
            "z-index",
            "zoom",

        };

        public static readonly HashSet<String> Keywords2 = new HashSet<String>()
        {
            "@require",
            "@import",
            "@media",
            "@font-face",
            "@keyframes",
            "@extend",
            "@block",
            "@viewport",
            "@supports",
            "@host",
            "@page"
        };
    }
}
