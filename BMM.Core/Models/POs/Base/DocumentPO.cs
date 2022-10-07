using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Base
{
    public abstract class DocumentPO : BasePO, IDocumentPO
    {
        private readonly Document _document;

        public DocumentPO(Document document)
        {
            _document = document;
        }

        public virtual DocumentType DocumentType => _document?.DocumentType ?? DocumentType.Unsupported;
        public int Id => _document?.Id ?? default;
    }
}