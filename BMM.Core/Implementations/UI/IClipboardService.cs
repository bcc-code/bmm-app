namespace BMM.Core.Implementations.UI
{
    public interface IClipboardService
    {
        /// <summary>
        /// Copies provided <c>textToCopy</c> to the clipboard.
        /// If <c>valueName</c> is provided, the information toast will be displayed
        /// </summary>
        void CopyToClipboard(string textToCopy, string valueName = "");
    }
}