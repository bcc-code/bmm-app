using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BMM.Core.Implementations.DeepLinking.Base;
using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking
{
    public class RegexDeepLink : DeepLinkParserBase, IDeepLinkParser
    {
        private readonly Func<Task> _action;

        private readonly string _regex;

        public RegexDeepLink(string regex, Func<Task> action)
        {
            _action = action;
            _regex = regex;
        }

        protected override bool CanNavigateTo(Uri uri, out Func<Task> action)
        {
            action = _action;

            Match match = new Regex(_regex).Match(uri.LocalPath);
            if (match.Success)
            {
                return true;
            }

            return false;
        }
    }
}