using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework.Exceptions;

namespace BMM.Api.Framework.HTTP
{
    public interface IBadRequestThrower
    {
        Task ThrowExceptionForBadRequest(HttpResponseMessage response);
    }

    public class BadRequestThrower : IBadRequestThrower
    {
        private readonly IResponseDeserializer _responseDeserializer;

        public BadRequestThrower(IResponseDeserializer responseDeserializer)
        {
            _responseDeserializer = responseDeserializer;
        }

        public async Task ThrowExceptionForBadRequest(HttpResponseMessage response)
        {
            var status = await _responseDeserializer.DeserializeResponse<ResponseStatus>(response);
            if (status.Errors?.Any() == true)
            {
                if (status.Errors.Any(e => e.StartsWith("TrackAlreadyInTrackCollection")))
                    throw new TrackAlreadyInTrackCollectionException();
                if (status.Errors.Any(e => e.StartsWith("AlbumAlreadyInTrackCollection")))
                    throw new AlbumAlreadyInTrackCollectionException();
            }
            throw new BadRequestException();
        }

    }
}
