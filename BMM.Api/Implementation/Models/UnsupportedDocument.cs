using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    /// <summary>
    /// This document is used when the document type is unknown. That can happen if the user is on an old version of the app and receives a new type from the API.
    /// </summary>
    [JsonObject]
    public class UnsupportedDocument : Document
    {
        public UnsupportedDocument()
        {
            DocumentType = DocumentType.Unsupported;
        }
    }
}