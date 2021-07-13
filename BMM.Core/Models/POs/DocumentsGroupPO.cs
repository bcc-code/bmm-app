using BMM.Api.Implementation.Models;
using BMM.Core.Models.Enums;
using BMM.Core.Models.POs.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.Models.POs
{
    public class DocumentsGroupPO : BasePO
    {
        public DocumentsGroupPO(DocumentGroupType type, MvxObservableCollection<Document> documents)
        {
            Type = type;
            Documents = documents;
        }

        public DocumentGroupType Type { get; }
        public MvxObservableCollection<Document> Documents { get; }
    }
}