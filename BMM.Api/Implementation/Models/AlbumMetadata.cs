using System;
using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class AlbumMetadata
    {
        public IEnumerable<string> ContainedTypes { get; set; }

        public bool IsVisible { get; set; }

        public DateTime ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }
    }
}