using System;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Core.Extensions
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task Run(this SemaphoreSlim semaphoreSlim, Func<Task> func)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                await func();
            }
            finally
            {
                semaphoreSlim.TryRelease();
            }
        }
        
        public static async Task<T> Run<T>(this SemaphoreSlim semaphoreSlim, Func<Task<T>> func)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return await func();
            }
            finally
            {
                semaphoreSlim.TryRelease();
            }
        }

        public static async Task Run(this SemaphoreSlim semaphoreSlim, Action action)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                action();
            }
            finally
            {
                semaphoreSlim.TryRelease();
            }
        }

        public static async Task<T> Run<T>(this SemaphoreSlim semaphoreSlim, Func<T> action)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                return action();
            }
            finally
            {
                semaphoreSlim.TryRelease();
            }
        }
        
        public static void TryRelease(this SemaphoreSlim semaphoreSlim)
        {
            try
            {
                semaphoreSlim?.Release();
            }
            catch (SemaphoreFullException)
            {
            }
        }
    }
}