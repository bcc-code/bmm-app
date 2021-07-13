using MvvmCross.ViewModels;

namespace BMM.Core.Models.POs.Base
{
    public abstract class BasePO : MvxNotifyPropertyChanged
    {
        protected BasePO()
        {
        }

        protected BasePO(string automationId) : this()
        {
            AutomationId = automationId;
        }

        public string AutomationId { get; protected set; }
    }
}