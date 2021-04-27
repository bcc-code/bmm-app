using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.DeepLinking
{
    public class RegexDeepLinkWithParameters : IDeepLinkParser
    {
        private readonly Func<int, string, Task> _actionWithParameters;
        private readonly string _regex;

        public RegexDeepLinkWithParameters(string regex, Func<int, string, Task> actionWithParameters)
        {
            _actionWithParameters = actionWithParameters;
            _regex = regex;
        }

        public bool CanNavigateTo(Uri uri, out Func<Task> action)
        {
            Match match = new Regex(_regex).Match(uri.LocalPath);

            var name = match.Groups["name"].Value;

            if (match.Success && int.TryParse(match.Groups["id"].Value, out var id))
            {
                action = () => _actionWithParameters.Invoke(id, name);
                return true;
            }

            action = null;
            return false;
        }
    }
}
