using System;

namespace BMM.Core.Implementations.FeatureToggles
{
    public class SemanticVersionParser
    {
        public SemanticVersion ParseStringToSemanticVersionObject(string version)
        {
            try
            {
                if(version.Contains("DEV"))
                {
                    return new SemanticVersion
                    {
                        Version = version,
                        Major = 999,
                        Minor = 999,
                        Patch = 999,
                        PreRelease = "dev",
                        Build = 999
                    };
                }

                return new SemanticVersion
                {
                    Version = version,
                    Major = ExtractMajor(version),
                    Minor = ExtractMinor(version),
                    Patch = ExtractPatch(version),
                    PreRelease = ExtractPreRelease(version),
                    Build = ExtractBuild(version, ExtractPreRelease(version))
                };
            }
            catch(Exception e)
            {
                throw new ArgumentException("Couldn't parse. Make sure provided version format meets the requirements of semantic version formatting.", e);
            }
        }

        private int ExtractMajor(string version)
        {
            string[] versionCore = version.Split('.');
            return versionCore.Length > 0 ? int.Parse(versionCore[0]) : 0;
        }

        private int ExtractMinor(string version)
        {
            string[] versionCore = version.Split('.');
            return versionCore.Length > 1 ? int.Parse(versionCore[1]) : 0;
        }

        private int ExtractPatch(string version)
        {
            string[] versionCore = version.Split('.');
            return versionCore.Length > 2 ? int.Parse(versionCore[2].Contains("-") ? versionCore[2].Remove(versionCore[2].IndexOf('-')) : versionCore[2]) : 0;
        }

        private string ExtractPreRelease(string version)
        {
            if (!version.Contains("-")) return "";
            if (version.Contains("alpha")) return "alpha";
            return version.Contains("beta") ? "beta" : "";
        }

        private int ExtractBuild(string version, string preRelease)
        {
            if (!version.Contains("-")) return 0;

            var major = ExtractMajor(version);
            var minor = ExtractMinor(version);
            var patch = ExtractPatch(version);

            var build = version
                .Replace(".", "")
                .Replace(major.ToString() + minor + patch, "")
                .Replace($"-{preRelease}", "");

            return build == "" ? 0 : int.Parse(build);
        }
    }
}
