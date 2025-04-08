using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Exceptions;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;
using KingdomRenderer.Shared.Zat;
using UnityEngine;
using static KingdomRenderer.Shared.ArchieV1.ULogger;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators
{
    /// <summary>
    /// A generator contains a collection of ResourceTypeBases and a method to edit the World map using these new
    /// ResourceTypeBases but also using built in ResourceTypes 
    /// </summary>
    public abstract class GeneratorBase
    {
        /// <summary>
        /// The name of this Generator.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The unique name of this Generator instance.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// The resources to be used in .Generate()
        /// </summary>
        public List<ModdedResourceType> Resources { get; }

        /// <summary>
        /// Gets or sets whether this Generator has been Registered.
        /// </summary>
        public bool Registered { get; set; }

        /// <summary>
        /// Create a ResourceTypeBase with multiple resources contained within it.
        /// Use this if generation code required multiple 
        /// </summary>
        /// <param name="moddedResourceTypes">The list of resources this generatorBase will use</param>
        public GeneratorBase(IEnumerable<ModdedResourceType> moddedResourceTypes)
        {
            ULog($"Generation", $"Creating {GetType()}.\n" +
                                               $"It contains the resources:\n\t{moddedResourceTypes.Join(null, "\n\t")}");

            //TODO check that this is true and doesnt just say "GeneratorBase"
            Name = GetType().ToString(); // The name of the class derived from this
            Guid = Guid.NewGuid();
            Resources = moddedResourceTypes.ToList();

            ULog($"Generation", $"Created {Name}");
        }

        /// <summary>
        /// Generates the resources in this generatorBase into the, already generated, world.
        /// Look at World.GenLand() for or http://www.ArchieV.uk/GoldMines for examples
        /// Please use World.inst.seed, SRand, and this.RandomStoneState for randomness.
        /// </summary>
        /// <param name="world"></param>
        /// <returns>Returns true if implemented</returns>
        public virtual bool Generate(World world)
        {
            throw new NotImplementedException("Generate needs to be implemented!");
        }

        public override string ToString()
        {
            return $"Geneartor: {Name}";
        }

        public string AllInfo()
        {
            StringBuilder str = new StringBuilder($"Generator: {Name}");
            str.Append("Resource Types:");
            foreach (ModdedResourceType x in Resources)
            {
                str.Append($"{x}");
            }

            return str.ToString();
        }

        public static void TryPlaceResource(Cell cell, ModdedResourceType resourceTypeBase,
            bool storePostGenerationType = false,
            bool deleteTrees = false,
            Vector3 localScale = new Vector3(),
            Vector3 rotate = new Vector3())
        {
            try
            {
                Debugging.Log($"Generation", $"Placing {resourceTypeBase} at {cell.x}, {cell.z}"); ;
                cell.Type = resourceTypeBase.ResourceType;
                GameObject gameObject = UnityEngine.Object.Instantiate(resourceTypeBase.Model);

                if (storePostGenerationType) cell.StorePostGenerationType();
                if (deleteTrees) TreeSystem.inst.DeleteTreesAt(cell);

                gameObject.transform.position = cell.Position;
                if (localScale != new Vector3()) gameObject.transform.localScale = localScale;
                if (rotate != new Vector3()) gameObject.transform.Rotate(rotate); // 0 or 180 for Z axis

                if (cell.Models == null)
                {
                    cell.Models = new List<GameObject>();
                }

                cell.Models.Add(gameObject);
                Debugging.Log($"Generation", "Placed");
            }
            catch (Exception e)
            {
                throw new PlacementFailedException(e.ToString());
            }
        }

        protected static bool ClearCell(Cell cell, bool clearCave = true)
        {
            if (cell.Models != null)
            {
                foreach (GameObject model in cell.Models)
                {
                    UnityEngine.Object.Destroy(model.gameObject);
                }

                cell.Models.Clear();
            }

            if (clearCave) ClearCaveAtCell(cell);

            return true;
        }

        protected static bool ClearCaveAtCell(Cell cell)
        {
            GameObject caveAt = World.inst.GetCaveAt(cell);
            if (caveAt != null)
            {
                UnityEngine.Object.Destroy(caveAt);
            }

            return true;
        }

        #region Useful Generation Methods
        // Places a small amount of "type" at x/y with some unusable stone around it
        // PlaceSmallStoneFeature(int x, int y, ResourceType type)

        // Places a large amount of "type" at x/y with some unusable stone around it
        // PlaceLargeStoneFeature(int x, int y, ResourceType type)

        // Sets cell at x/y's type to be "type".
        // Deleted the trees at that location
        // Calls SetupStoneForCell(Cell cell) to instantiate the model
        // PlaceStone(int x, int y, ResourceType type)

        // Instantiates the model and 
        // gameObject = UnityEngine.Object.Instantiate<GameObject>(CORRECT_MODEL); // Instantiate the mode
        // gameObject.GetComponent<MeshRenderer>().sharedMaterial = this.uniMaterial[0]; // Apply the mesh. Always [0]. What is uniMaterial
        // gameObject.transform.parent = this.resourceContainer.transform; // Smth to do with moving it relative to the parent but not in the world
        // SetupStoneForCell(Cell cell)
        #endregion
    }
}
