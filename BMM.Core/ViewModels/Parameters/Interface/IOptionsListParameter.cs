using System.Collections.Generic;
using BMM.Core.Models.POs;

namespace BMM.Core.ViewModels.Parameters.Interface
{
    public interface IOptionsListParameter
    {
        IList<StandardIconOptionPO> Options { get; }
    }
}