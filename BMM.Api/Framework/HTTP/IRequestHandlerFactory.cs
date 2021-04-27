namespace BMM.Api.Framework.HTTP
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler CreateInstance(IRequestInterceptor requestInterceptor);

        IRequestHandler BuildUnauthorizedRequestHandler();

        IRequestHandler BuildRequestHandler();
    }
}