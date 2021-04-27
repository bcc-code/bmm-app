namespace BMM.Core.Implementations.FeatureToggles
{
    public class SemanticVersion
    {
        public string Version { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public string PreRelease { get; set; }

        public int Build { get; set; }
    }
}
