using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tracks.Interfaces;

public interface IHighlightedTextPO : IDocumentPO
{
    string Text { get; }
    long StartPositionInMs { get; }
    IMvxAsyncCommand ItemClickedCommand { get; }
    SearchHighlight SearchHighlight { get; }
}