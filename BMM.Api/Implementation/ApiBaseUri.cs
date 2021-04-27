using System;

namespace BMM.Api.Implementation
{
    public class ApiBaseUri : Uri
    {
        public ApiBaseUri(string uriString) : base(uriString)
        { }
    }
}
