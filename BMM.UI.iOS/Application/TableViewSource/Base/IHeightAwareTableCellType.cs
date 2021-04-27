using System;

namespace BMM.UI.iOS
{
    public interface IHeightAwareTableCellType: ITableCellType
    {
        nfloat Height { get; }
    }
}