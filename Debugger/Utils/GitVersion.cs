// <copyright file="GitVersion.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace ModTools.Utils
{
    /// <summary>
    /// A helper class that interacts with the special types injected by the GitVersion toolset.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Git", Justification = "Reviewed")]
    public static class GitVersion
    {
        private const string GitVersionTypeName = "GitVersionInformation";
        private const string VersionFieldName = "FullSemVer";

        /// <summary>
        /// Gets a string representation of the full semantic assembly version of the specified <paramref name="assembly"/>.
        /// This assembly should be built using the GitVersion toolset; otherwise, a "?" version string will
        /// be returned.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
        ///
        /// <param name="assembly">An <see cref="Assembly"/> to get the version of. Should be built using the GitVersion toolset.</param>
        ///
        /// <returns>A string representation of the full semantic version of the specified <paramref name="assembly"/>,
        /// or "?" if the version could not be determined.</returns>
        public static string GetAssemblyVersion(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var gitVersionInformationType =
                assembly.GetType(GitVersionTypeName)
                ?? assembly.GetType($"{assembly.GetName().Name}.{GitVersionTypeName}");

            if (gitVersionInformationType == null)
            {
                Logger.Error("Attempting to retrieve the assembly version of an assembly that is built without GitVersion support.");
                return "?";
            }

            var versionField = gitVersionInformationType.GetField(VersionFieldName);
            if (versionField == null)
            {
                Logger.Error($"Internal error: the '{GitVersionTypeName}' type has no field '{VersionFieldName}'.");
                return "?";
            }

            try
            {
                var version = versionField.GetValue(null) as string;
                if (string.IsNullOrEmpty(version))
                {
                    Logger.Warning($"The '{GitVersionTypeName}.{VersionFieldName}' value is empty.");
                    return "?";
                }

                return version;
            }
            catch (TargetException)
            {
                Logger.Warning($"The API of GitVersion has changed: '{GitVersionTypeName}.{VersionFieldName}' is not static.");
                return "?";
            }
            catch (FieldAccessException)
            {
                Logger.Warning($"We are in restricted security context, the field '{GitVersionTypeName}.{VersionFieldName}' cannot be accessed.");
                return "?";
            }
        }
    }
}
