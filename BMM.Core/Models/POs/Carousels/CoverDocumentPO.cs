using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Carousels
{
    public class CoverDocumentPO : DocumentPO
    {
        public CoverDocumentPO(Document coverDocument) : base(coverDocument)
        {
            CoverDocument = (ITrackListDisplayable)coverDocument;
        }
        
        public ITrackListDisplayable CoverDocument { get; }
    }
}