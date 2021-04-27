using System.Text.RegularExpressions;

namespace BMM.Core.Implementations.Validators
{
    public class MailValidator : IMailValidator
    {
        private Regex regex;
        private string regexExpresion = "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

        public MailValidator()
        {
            regex = new Regex(regexExpresion);
        }

        public bool ValidateMail(string mail)
        {
            var match = regex.Match(mail.ToLower());
            if (!match.Success)
            {
                return false;
            }

            return true;
        }
    }
}
