using System;

namespace BMM.UI.iOS
{
    public class HeightAwareTableCellType: TableCellType, IHeightAwareTableCellType
    {
        public HeightAwareTableCellType(Type itemType, string uibName, nfloat height) : base(itemType, uibName)
        {
            Height = height;
        }

        public nfloat Height { get; }
    }
}