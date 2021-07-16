using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BMM.Core.Implementations.DeepLinking.Base;
using BMM.Core.Implementations.DeepLinking.Base.Interfaces;

namespace BMM.Core.Implementations.DeepLinking
{
    public class RegexDeepLink<TParameters>
        : DeepLinkParserBase,
          IDeepLinkParser where TParameters : IDeepLinkParameters
    {
        private readonly Func<TParameters, Task> _actionWithParameters;
        private readonly string _regex;

        public RegexDeepLink(string regex, Func<TParameters, Task> actionWithParameters)
        {
            _actionWithParameters = actionWithParameters;
            _regex = regex;
        }

        protected override bool CanNavigateTo(Uri uri, out Func<Task> action)
        {
            Match match = new Regex(_regex).Match(uri.LocalPath);

            if (!match.Success)
            {
                action = null;
                return false;
            }

            var parametersType = _actionWithParameters
                .GetType()
                .GenericTypeArguments
                .First();

            var parametersTypeInstance = Activator.CreateInstance(parametersType);

            foreach (var property in parametersType.GetProperties())
            {
                if (!match.Groups[property.Name.ToLower()].Success)
                    continue;

                property.SetValue(parametersTypeInstance, Convert.ChangeType(match.Groups[property.Name.ToLower()].Value, property.PropertyType));
            }

            action = () => _actionWithParameters.Invoke((TParameters)parametersTypeInstance);
            return true;
        }
    }
}