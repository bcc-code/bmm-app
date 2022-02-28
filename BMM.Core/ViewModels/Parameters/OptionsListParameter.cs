using System.Collections.Generic;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters
{
    public class OptionsListParameter : IOptionsListParameter
    {
        public OptionsListParameter(IList<StandardIconOptionPO> options)
        {
            Options = options;
        }
        
        public IList<StandardIconOptionPO> Options { get; }
    }
}