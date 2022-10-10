using System;
using System.Threading.Tasks;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.DeepLinks.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;
using BMM.Core.Helpers;

namespace BMM.Core.GuardedActions.DeepLinks
{
    public class InternalDeepLinkAction
        : GuardedActionWithParameter<InternalDeepLinkActionParameter>,
          IInternalDeepLinkAction
    {
        private readonly IDeepLinkHandler _deepLinkHandler;

        public InternalDeepLinkAction(IDeepLinkHandler deepLinkHandler)
        {
            _deepLinkHandler = deepLinkHandler;
        }
        
        protected override Task Execute(InternalDeepLinkActionParameter parameter)
        {
            _deepLinkHandler.OpenFromInsideOfApp(new Uri(parameter.Link), parameter.Origin);
            return Task.CompletedTask;
        }
    }
}