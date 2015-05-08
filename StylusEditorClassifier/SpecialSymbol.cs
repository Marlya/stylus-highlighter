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
        public List<String> StringNotStartsWith { get; set; }
        /// <summary>
        /// A list of states in which symbol is valid. If current state is not in this list Symbol is not valid.
        /// </summary>
        public List<State> ValidStates { get; set; }
        /// <summary>
        /// A list of states in which symbol is not valid.
        /// </summary>
        public List<State> NotValidStates { get; set; }
    }

    internal enum IncludeType
    {
        Exclude = 0,
        IncludeToLeft = 1,
        IncludeToRight = 2
    }
}
