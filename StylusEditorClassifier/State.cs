namespace StylusEditorClassifier
{
    enum State
    {
        None = -1,
        Default = 0,
        IsComment = 1,
        IsMultiComment = 2,
        AfterKeyword = 3,
        AfterKeywordAfterBracket = 4,
        Bracket = 5
    }
}
