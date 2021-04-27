using System.Collections.Generic;

namespace BMM.Api.Framework.HTTP
{
    public class ResponseStatus
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public IList<string> Errors { get; set; }
    }
}