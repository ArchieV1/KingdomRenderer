using System.Collections.Generic;
using KingdomRenderer.Shared.ArchieV1.System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources
{
    public class ModdedResourceType
    {
        /// <summary>
        /// If this is a mirror of a built-in game <see cref="ResourceType"/>
        /// </summary>
        public bool DefaultResource { get; } = false;

        /// <summary>
        /// The name of this resource
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The model this resources uses in the game world
        /// </summary>
        [JsonIgnore] // TODO unsure if needed
        public GameObject Model { get; private set; }

        /// <summary>
        /// Gets the path of the AssetBundle
        /// </summary>
        public string AssetBundlePath { get; }

        /// <summary>
        /// Gets or sets hoSw many tiles away a Cave or Witch must be from this resource.
        /// 0 means Witches and Caves can be placed next to this resource.
        /// </summary>
        // Stone/Iron is 2 tiles
        // EmptyCave/Structure is 5 tiles
        // TODO implement this 
        public int CaveWitchMustBePlacedXTilesAway { get; set; } = 2;

        /// <summary>
        /// Gets or sets how many tiles with trees must surround the Resource for it to be placed within the given range.
        /// </summary>
        // Cave/Witch required 4 trees in a radius of 1. (4 of out 8)
        // TODO implement this 
        public TreeRequirement? NumberTreesRequiredNearby { get; set; } = null;

        /// <summary>
        /// The name of the Prefab
        /// </summary>
        public string PrefabName { get; }

        /// <summary>
        /// The AssetBundle
        /// </summary>
        [JsonIgnore] // TODO unsure if needed
        public AssetBundle AssetBundle { get; private set; }

        /// <summary>
        /// If the Model is loaded
        /// </summary>
        public bool ModelLoaded => Model != null;

        /// <summary>
        /// The ResourceType that this mirrors
        /// Can be either a built-in ResourceType or an INT
        /// 
        /// This should not be set by modders. It will be set by the ModdingFramework upon all mods being registered
        /// </summary>
        public ResourceType ResourceType { get; set; } = ResourceType.None;

        /// <summary>
        /// If true will not assign this a ResourceType other than ResourceType.None.
        /// </summary>
        public bool DoNotAssignResourceType { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this ModdedResourceType has been registered by the KCModdingFramework.
        /// </summary>
        public bool Registered { get; set; } = false;


        /// <summary>
        /// The way(s) the resource can be desroyed (Eg Rock destroyer, axe tool)
        /// </summary>
        public IEnumerable<ResourceDestructionMethods> DestructionMethods { get; }

        /// <summary>
        /// The ResourceTypeBase to appear on the map.
        /// Generated using a Generator which is part of a ModConfigMF.
        /// </summary>
        /// <param name="name">The name of the ResourceTypeBase.</param>
        /// <param name="model">The model to be used on the map.</param>
        public ModdedResourceType(string name, GameObject model)
        {
            Name = name;
            Model = model;
        }

        /// <summary>
        /// The ResourceTypeBase to appear on the map.
        /// Generated using a Generator which is part of a ModCOnfigMF.
        /// </summary>
        /// <param name="name">The name of the new Resource.</param>
        /// <param name="assetBundlePath">The AssetBundle path.</param>
        /// <param name="prefabName">The name of the prefab.</param>
        public ModdedResourceType(string name, string assetBundlePath, string prefabName)
        {
            Name = name;
            AssetBundlePath = assetBundlePath;
            PrefabName = prefabName;
        }

        /// <summary>
        /// Creates a new instance of ResourceTypeBase
        /// </summary>
        public ModdedResourceType()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override string ToString()
        {
            return $"{Name}\n" +
                   $"Model loaded: {ModelLoaded}\n" +
                   $"AssetBundlePath: {AssetBundlePath}\n" +
                   $"ResourceType: {ResourceType}";
        }

        /// <summary>
        /// Loads the asset bundle into the memory of this object.
        /// WARNING: Make sure to run this BEFORE <see cref="LoadModel"/>.
        /// </summary>
        /// <param name="helper"></param>
        public void LoadAssetBundle(KCModHelper helper)
        {
            AssetBundle = KCModHelper.LoadAssetBundle(helper.modPath, AssetBundlePath);
        }

        /// <summary>
        /// Loads the model from <see cref="AssetBundle"/>.
        /// WARNING: Make sure to have run  <see cref="LoadAssetBundle(KCModHelper)"/> beforehand.
        /// </summary>
        public void LoadModel()
        {
            Model = AssetBundle.LoadAsset(Path.Join(AssetBundlePath, PrefabName)) as GameObject;
        }
    }
}
