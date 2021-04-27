using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Helpers
{
    public interface IShareLink
    {
        Task For(Track track);
        Task For(Album album);
        Task For(Contributor contributor);
    }
}
