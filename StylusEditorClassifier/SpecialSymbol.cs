using System;
using System.Collections.Generic;

namespace StylusEditorClassifier
{
    /// <summary>
    /// Symbols like: /*, */, //, :, (, etc.
    /// </summary>
    class SpecialSymbol
    {
        public String Symbol { get; set; }
        public IncludeType Include { get; set; }
        public Boolean StartsWithZero { get; set; }
        /// <summary>
        /// A collections of string beginnings which indicate that this symbol is not a special symbol.
        /// </summary>
        public List<String> UnsuitableStringBeginnigns { get; set; }
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
