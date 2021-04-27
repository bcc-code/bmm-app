using BMM.Core.Implementations.FileStorage;

namespace BMM.Core.Implementations.Exceptions
{
    public class StorageOutOfSpaceException: System.Exception
    {
        private IFileStorage _storage;
        public StorageOutOfSpaceException(IFileStorage storage): base()
        {
            Initialization(storage);
        }

        public StorageOutOfSpaceException(string message, IFileStorage storage) : base(message)
        {
            Initialization(storage);
        }

        public IFileStorage Storage => _storage;

        private void Initialization(IFileStorage storage)
        {
            _storage = storage;
        }
    }
}
