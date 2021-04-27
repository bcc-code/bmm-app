namespace BMM.Api.Framework
{
    public interface IToken
    {
        string Username { get; }
        string AuthenticationToken { get; }
    }
}