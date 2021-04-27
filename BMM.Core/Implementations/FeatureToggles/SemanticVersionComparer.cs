namespace BMM.Core.Implementations.FeatureToggles
{
    public class SemanticVersionComparer
    {
        public bool SatisfiesMinVersion(SemanticVersion semanticVersionInQuestion, SemanticVersion minVersion)
        {
            return GreaterThanOrEqual(semanticVersionInQuestion, minVersion);
        }

        public bool LessThanOrEqual(SemanticVersion semanticVersionInQuestion, SemanticVersion minVersion)
        {
            if (semanticVersionInQuestion.Major > minVersion.Major) return false;
            if (semanticVersionInQuestion.Major < minVersion.Major) return true;

            if (semanticVersionInQuestion.Major == minVersion.Major)
            {
                if (semanticVersionInQuestion.Minor > minVersion.Minor) return false;
                if (semanticVersionInQuestion.Minor < minVersion.Minor) return true;

                if (semanticVersionInQuestion.Minor == minVersion.Minor)
                {
                    if (semanticVersionInQuestion.Patch > minVersion.Patch) return false;
                    if (semanticVersionInQuestion.Patch < minVersion.Patch) return true;
                }
            }

            return true;
        }

        public bool GreaterThanOrEqual(SemanticVersion semanticVersionInQuestion, SemanticVersion minVersion)
        {
            if (semanticVersionInQuestion.Major > minVersion.Major) return true;
            if (semanticVersionInQuestion.Major < minVersion.Major) return false;

            if (semanticVersionInQuestion.Major == minVersion.Major)
            {
                if (semanticVersionInQuestion.Minor > minVersion.Minor) return true;
                if (semanticVersionInQuestion.Minor < minVersion.Minor) return false;

                if (semanticVersionInQuestion.Minor == minVersion.Minor)
                {
                    if (semanticVersionInQuestion.Patch > minVersion.Patch) return true;
                    if (semanticVersionInQuestion.Patch < minVersion.Patch) return false;
                }
            }

            return true;
        }
    }
}
