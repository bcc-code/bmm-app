using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class User
    {
        public string FirstName { get; set; }

        public string FullName => FirstName + " " + LastName;

        public string LastName { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public string Token { get; set; }

        public IList<TrackCollection> TrackCollections { get; set; }

        public string Username { get; set; }

        public int? PersonId { get; set; }

        /// <summary>
        /// This url is taken from the id_token.
        /// But since it's set in the login phase it won't be set for most users. They would need to relogin for this to be set.
        /// We decided against finding a solution without relogin, since we believe this picture is a minor feature and won't have much impact.
        /// Also, we expect most users not to know where they can change the picture.
        /// Therefore we decided to use our resources somewhere else.
        /// </summary>
        public string ProfileImage { get; set; }

        public string AnalyticsId { get; set; }
    }
}