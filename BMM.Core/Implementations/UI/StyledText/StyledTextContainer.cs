using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Implementations.UI.StyledText.Interfaces;

namespace BMM.Core.Implementations.UI.StyledText;

public class StyledTextContainer
{
    public StyledTextContainer(
        IEnumerable<IStyledText> texts,
        int fontSize,
        bool shouldColorBackground,
        float lineHeightMultiplier)
    {
        FontSize = fontSize;
        ShouldColorBackground = shouldColorBackground;
        LineHeightMultiplier = lineHeightMultiplier;
        foreach (var text in texts)
            StyledTexts.Add(text);
    }
    
    public IList<IStyledText> StyledTexts { get; } = new List<IStyledText>();
    public int FontSize { get; }
    public bool ShouldColorBackground { get; }
    public float LineHeightMultiplier { get; } 

    public string FullText =>
        string.Join(
            null,
            StyledTexts.Select(s => s.Content));
}