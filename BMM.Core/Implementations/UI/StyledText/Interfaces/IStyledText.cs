using BMM.Core.Implementations.UI.StyledText.Enums;

namespace BMM.Core.Implementations.UI.StyledText.Interfaces;

public interface IStyledText
{
    TextStyle Style { get; }
    string Content { get; }
    int Position { get; }
    int Length { get; }
}