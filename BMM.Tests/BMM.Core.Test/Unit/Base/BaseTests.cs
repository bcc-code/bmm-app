using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Base
{
    public abstract class BaseTests
    {
        [SetUp]
        public virtual async Task SetUp()
        {
            await PrepareMocks();
            InitializeTestSubject();
            await AfterTestSubjectInitialization();
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            await ClearMocks();
        }

        private protected abstract void InitializeTestSubject();

        protected virtual Task AfterTestSubjectInitialization()
        {
            return Task.CompletedTask;
        }

        protected virtual Task PrepareMocks()
        {
            return Task.CompletedTask;
        }

        protected virtual Task ClearMocks()
        {
            return Task.CompletedTask;
        }

        protected static IList<T> CreateList<T>(int count = 0, Action<int, T> prepareItem = null)
            where T : class
        {
            var list = new List<T>();

            for (int i = 0; i < count; i++)
            {
                var item = Substitute.For<T>();
                prepareItem?.Invoke(i, item);
                list.Add(item);
            }

            return list;
        }
    }
}