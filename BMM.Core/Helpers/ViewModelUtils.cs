using System;

namespace BMM.Core.Helpers
{
    public class ViewModelUtils
    {
        public static string GetVMTitleKey(Type vmType) => $"{vmType.Name}_Title";
    }
}