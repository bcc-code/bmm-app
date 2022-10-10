using BMM.Api.Implementation.Models;

namespace BMM.Core.Models.POs.Base.Interfaces
{
    public interface IDocumentPO : IBasePO
    {
        public DocumentType DocumentType { get; }
        public int Id { get; }
    }
}