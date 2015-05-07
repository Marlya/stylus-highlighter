using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StylusEditorClassifier
{
    class SpecialSymbol
    {
        public String Symbol { get; set; }
        public IncludeType Include { get; set; }
        public Boolean StartsWithZero { get; set; }
    }

    internal enum IncludeType
    {
        Exclude = 0,
        IncludeToLeft = 1,
        IncludeToRight = 2
    }
}
