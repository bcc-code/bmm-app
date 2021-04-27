using System.Collections.Generic;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class ContentLanguagesChangedMessage: MvxMessage
    {
        public ContentLanguagesChangedMessage (object sender, IEnumerable<string> languages)
            : base(sender)
        {
            Languages = languages;
        }

        public readonly IEnumerable<string> Languages;
    }
}

