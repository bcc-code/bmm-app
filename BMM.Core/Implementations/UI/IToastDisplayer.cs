using System;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.UI
{
    public interface IToastDisplayer
    {
        Task Success(string message);

        [Obsolete]
        void Warn(string message);
        Task WarnAsync(string message);

        [Obsolete]
        void Error(string message);
        Task ErrorAsync(string message);
    }
}