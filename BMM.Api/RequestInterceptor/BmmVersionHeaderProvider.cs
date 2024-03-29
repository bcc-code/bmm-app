﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Constants;

namespace BMM.Api.RequestInterceptor
{
    public class BmmVersionHeaderProvider : IHeaderProvider
    {
        private readonly IBmmVersionProvider _bmmVersionProvider;

        public BmmVersionHeaderProvider(IBmmVersionProvider bmmVersionProvider)
        {
            _bmmVersionProvider = bmmVersionProvider;
        }

        public async Task<KeyValuePair<string, string>?> GetHeader()
        {
            return new KeyValuePair<string, string>(HeaderNames.BmmVersion, _bmmVersionProvider.BmmVersion);
        }
    }
}