using System;

namespace BMM.UI.iOS
{
    public class TableCellType: ITableCellType
    {
        public TableCellType(Type itemType, string uibName)
        {
            ItemType = itemType;
            UibName = uibName;
        }

        public Type ItemType { get; }
        public string UibName { get; }
    }
}