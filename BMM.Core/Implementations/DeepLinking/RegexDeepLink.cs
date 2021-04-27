using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.DeepLinking
{
    public interface IDeepLinkParser
    {
        bool CanNavigateTo(Uri uri, out Func<Task> action);
    }

    public class RegexDeepLink : IDeepLinkParser
    {
        private readonly Func<Task> _action;

        private readonly string _regex;

        public RegexDeepLink(string regex, Func<Task> action)
        {
            _action = action;
            _regex = regex;
        }

        public bool CanNavigateTo(Uri uri, out Func<Task> action)
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