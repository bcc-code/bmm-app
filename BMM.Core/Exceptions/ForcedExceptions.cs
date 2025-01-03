namespace BMM.Core.Exceptions
{
    public class ForcedException : Exception
    {
        public ForcedException(object sender) : base(sender.ToString())
        {
        }
    }
}