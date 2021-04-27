using BMM.Core.Implementations.FileStorage;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class SelectedStorageChangedMessage : MvxMessage
    {
        private readonly IFileStorage _fileStorage;

        public SelectedStorageChangedMessage(object sender, IFileStorage selectedFileStorage)
            : base(sender)
        {
            _fileStorage = selectedFileStorage;
        }

        public IFileStorage FileStorage => _fileStorage;
    }
}
