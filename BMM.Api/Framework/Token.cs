namespace BMM.Api.Framework
{
    public class Token : IToken
    {
        public Token(string username, string authenticationToken)
        {
            Username = username;
            AuthenticationToken = authenticationToken;
        }

        public string AuthenticationToken { get; }

        public string Username { get; }
    }
}