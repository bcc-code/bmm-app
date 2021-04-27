using System;

namespace BMM.UI.iOS
{
    public interface ITableCellType
    {
        Type ItemType { get; }
        string UibName { get; }
    }
}