using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs
{
    public class HeaderPO : BasePO
    {
        private string _header;

        public HeaderPO(string header)
        {
            Header = header;
        }
        
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }
    }
}