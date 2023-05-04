using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Implementations.UI.StyledText.Interfaces;

namespace BMM.Core.Implementations.UI.StyledText;

public class StyledText : IStyledText
{
    public StyledText(TextStyle style, string content, int position)
    {
        Style = style;
        Content = content;
        Position = position;
    }
    
    public TextStyle Style { get; }
    public string Content { get; }
    public int Position { get; }
    public int Length => Content.Length;
}