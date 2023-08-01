using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Other.Interfaces;

public interface ISelectableListContentItemPO : IBasePO
{
    string Title { get; }
    string Text { get; }
    IMvxCommand OnSelected { get; }
}