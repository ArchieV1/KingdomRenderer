using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;
using UnityEngine;
using static KingdomRenderer.Shared.ArchieV1.ULogger;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.ModConfig
{
    /// <summary>
    /// The information for mods being registered.
    /// This should be sent to the KCModdingFramework by modders.
    /// </summary>
    public class ModConfigMF
    {
        /// <summary>
        /// Name for logging from this ModConfigMF
        /// </summary>
        private string cat => $"ModConfigMF - {ModName}";

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// The author of the mod. E.g. greenking2000.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The Generators this mod uses to add new items to the map.
        /// </summary>
        public ICollection<GeneratorBase> Generators { get; set; }

        /// <summary>
        /// The asset bundles this mod uses for its resources.
        /// </summary>
        public ICollection<AssetBundle> AssetBundles { get; set; }

        /// <summary>
        /// Gets or sets the dependencies of this mod. Defined by the <see cref="ModName"/> of the mods.
        /// Each dependency is lower case and unique.
        /// </summary>
        public IEnumerable<string> Dependencies
        { 
            get
            {
                return Dependencies ?? new List<string>(); ;
            }
            set 
            {
                IEnumerable<string> newDependencies = Dependencies.Union(value);
                Dependencies = new List<string>();
                this.AddDependencies(newDependencies); 
            }
        }

        /// <summary>
        /// If this mod has been registered with the KCModdingFramework.
        /// Should be `false` when it is sent to KCMF and `true` when returned.
        /// </summary>
        public bool Registered { get; set; } = false;

        /// <summary>
        /// Extra ModdedResourceTypes that none of the <see cref="Generators"/> use.
        /// </summary>
        public ICollection<ModdedResourceType> ExtraModdedResourceTypes { get; set; }

        /// <summary>
        /// All of the ModdedResourceTypes this Mod's <see cref="Generators"/> will use. (And <see cref="ExtraModdedResourceTypes"/>)
        /// This is calculated by reading the Generators. A ResourceType can be used by multiple generators.
        /// </summary>
        public IEnumerable<ModdedResourceType> ModdedResourceTypes
        {
            get
            {
                return Generators.SelectMany(g => g.Resources).Union(ExtraModdedResourceTypes).Distinct();
            }
        }

        /// <summary>
        /// Adds given <paramref name="modName"/> to the <see cref="Dependencies"/> list.
        /// Caps insensitive.
        /// </summary>
        /// <param name="modName">The modname to add.</param>
        public void AddDependency(string modName)
        {
            if (string.IsNullOrEmpty(modName))
            {
                throw new ArgumentException("Dependency names cannot be null or empty");
            }

            modName = modName.ToLowerInvariant();

            if (!Dependencies.Contains(modName))
            {
                Dependencies.Add(modName);
                Dependencies = Dependencies.Distinct();
            }
            else
            {
                ULog(cat, $"Adding dependency {modName} though it was already added!");
            }

        }

        /// <summary>
        /// Adds name of <paramref name="modConfig"/> to the <see cref="Dependencies"/> list.
        /// </summary>
        /// <param name="modConfig">The modconfig to add.</param>
        public void AddDependency(ModConfigMF modConfig)
        {
            AddDependency(modConfig.ModName);
        }

        /// <summary>
        /// Adds all given <paramref name="modNames"/> to the <see cref="Dependencies"/> list.
        /// </summary>
        /// <param name="modConfig">The modNames to add.</param>
        public void AddDependencies(IEnumerable<string> modNames)
        {
            foreach (string dependency in modNames)
            {
                AddDependency(dependency);
            }
        }

        /// <summary>
        /// Adds the names of all given <paramref name="dependencies"/> to the <see cref="Dependencies"/> list.
        /// </summary>
        /// <param name="modConfig">The modNames to add.</param>
        public void AddDependencies(IEnumerable<ModConfigMF> dependencies)
        {
            foreach (ModConfigMF dependency in dependencies)
            {
                AddDependency(dependency);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="modName"/> from the <see cref="Dependencies"/> list.
        /// </summary>
        /// <param name="modName">The name of the dependency to remove.</param>
        [Obsolete("There is no reason I can think of to remove a dependency in code. Please reconsider if this is correct.")]
        public void RemoveDependency(string modName)
        {
            List<string> dependenciesCopy = Dependencies.ToList();
            dependenciesCopy.Remove(modName.ToLowerInvariant());

            Dependencies = dependenciesCopy;
        }

        public bool IsDependency(string modName)
        {
            return Dependencies.Contains(modName.ToLowerInvariant());
        }

        public bool IsNotDependency(string modName)
        {
            return !IsDependency(modName);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override string ToString()
        {
            string generatorNames = string.Join(", ", Generators.Select(g => $"{g.Name}[{g.Guid}]"));

            return $"{ModName} by {Author}. Generators included are {generatorNames}";
        }
    }
}
