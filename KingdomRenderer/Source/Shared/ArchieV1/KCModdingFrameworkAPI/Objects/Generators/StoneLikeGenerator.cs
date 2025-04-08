using System.Collections.Generic;
using System.Linq;
using KingdomRenderer.Shared.ArchieV1.Extensions.KCExtensions;
using KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Resources;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Objects.Generators
{
    /// <summary>
    /// Use this Generator if you wish your modded resource to spawn like vanilla Stone does. Requires two resourceTypes
    /// </summary>
    public class StoneLikeGenerator : GeneratorBase
    {
        private List<ModdedResourceType> _resourceTypeBases;
        private bool _largeFeature = false;
        private int[] _mapSizeBiases;

        private struct MapFeatureDef
        {
            // Taken from `World.MapFeatureDef` though feature doesn't seem needed
            // public World.MapFeature feature; 
            public int x;
            public int z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceTypeBases"></param>
        /// <param name="largeFeature"></param>
        /// <param name="ironMode">If true changes mapSizeBiases default to follow the numbers for iron instead of stone. No effect with custom mapSizeBiases</param>
        /// <param name="mapSizeBiases">How many extra resource deposits to place per map size. Small, Medium, Large. Default {1, 2, 4}</param>
        public StoneLikeGenerator(IEnumerable<ModdedResourceType> resourceTypeBases, bool largeFeature = false, bool ironMode = false, int[] mapSizeBiases = null) : base(resourceTypeBases)
        {
            // Transforms ResourceTypeBase to either
            // ResourceType1 + UnusableStone
            // ResourceType1 + ResourceType2
            switch (resourceTypeBases.Count())
            {
                case 1:
                    _resourceTypeBases = new List<ModdedResourceType>
                        {
                            resourceTypeBases.First(),
                            new ModdedResourceType
                            {
                                ResourceType = ResourceType.UnusableStone
                            }
                        };
                    break;
                case 2:
                    _resourceTypeBases = resourceTypeBases.ToList();
                    break;
                default:
                    _resourceTypeBases = resourceTypeBases.Take(2).ToList();
                    break;
            };

            _largeFeature = largeFeature;

            if (mapSizeBiases != null && mapSizeBiases.Length == 3)
            {
                _mapSizeBiases = mapSizeBiases;
            }
            else
            {
                _mapSizeBiases = ironMode ? new[] { 1, 1, 2 } : new[] { 1, 2, 4 };
            }
        }

        /// <summary>
        /// Stone generation requires two resources.
        /// A good resource and a dud resource. Can use vanilla resources for either. Just use their data in your ResourceTypeBase implementation
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public override bool Generate(World world)
        {
            // Edited version of `TryPlaceResource`: 2022/09/14
            // Recreating values of params
            // private void TryPlaceResource(World.MapFeature feature, ArrayExt<Cell> cellsToLandmassFiltered, List<World.MapFeatureDef> placedFeatures)

            // feature
            // The "feature" (Iron deposit, Fertile ground etc) to place
            // Not needed

            // placedFeatures
            // A list of a currently placed features. Seems to be empty each time it is given to `TryPlaceResource` though.
            // So the game only makes sure resources of the same type arent near each other? Different types can spawn next to each other?
            List<MapFeatureDef> placedFeatures = new List<MapFeatureDef>();


            // cellsToLandmassFiltered
            // ?? A list of cells comprising each island?
            ArrayExt<Cell>[] array = new ArrayExt<Cell>[world.NumLandMasses];
            for (int k = 0; k < world.NumLandMasses; k++)
            {
                Cell[] cellData = (Cell[])WorldExtension.GetPrivateField(world, "cellData", fieldIsStatic: false);
                array[k] = new ArrayExt<Cell>(cellData.Length);
                for (int l = 0; l < world.cellsToLandmass[k].Count; l++)
                {
                    if (world.cellsToLandmass[k].data[l].Type != ResourceType.Water)
                    {
                        array[k].Add(world.cellsToLandmass[k].data[l]);
                    }
                }
            }

            // For each island
            for (int num3 = 0; num3 < world.NumLandMasses; num3++)
            {
                ArrayExt<Cell> cellsToLandmassFiltered = array[num3];

                // Make different amounts of resources generate based on map size
                int mapSizeBias = 0;
                if (world.generatedMapsBias == World.MapBias.Land)
                {
                    if (world.generatedMapSize == World.MapSize.Small)
                    {
                        mapSizeBias = _mapSizeBiases[0];
                    }
                    else if (world.generatedMapSize == World.MapSize.Medium)
                    {
                        mapSizeBias = _mapSizeBiases[1];
                    }
                    else if (world.generatedMapSize == World.MapSize.Large)
                    {
                        mapSizeBias = _mapSizeBiases[2];
                    }
                }

                int index2 = 0;  // Seems to just always be 0
                for (int depositNumber = 0; depositNumber < world.baseDensities[index2].stone + mapSizeBias; depositNumber++)
                {
                    // this.TryPlaceResource(World.MapFeature.SmallStone, array[num3], placedFeatures);

                    // Edited implementation of the above method. Does not have `placedFeatures` as that is not stored anywhere
                    bool flag = false;
                    int num = 50;
                    while (!flag && num > 0)
                    {
                        Cell cell = cellsToLandmassFiltered.RandomElement();
                        bool flag2 = false;
                        for (int i = 0; i < placedFeatures.Count; i++)
                        {
                            float xCoordDelta = Mathff.Abs(placedFeatures[i].x - cell.x);
                            float zCoordDelta = Mathff.Abs(placedFeatures[i].z - cell.z);
                            if (xCoordDelta < 4f && zCoordDelta < 4f)
                            {
                                flag2 = true;
                                break;
                            }
                        }

                        if (!flag2)
                        {
                            switch (_largeFeature)
                            {
                                case true:
                                    flag = WorldExtension.PlaceLargeStoneFeature(world, cell, _resourceTypeBases[0].ResourceType, _resourceTypeBases[1].ResourceType, 60, 35);
                                    break;
                                default:
                                    flag = WorldExtension.PlaceSmallStoneFeature(world, cell, _resourceTypeBases[0].ResourceType, _resourceTypeBases[0].ResourceType, 60, 35);
                                    break;
                            }

                            if (flag)
                            {
                                placedFeatures.Add(new MapFeatureDef
                                {
                                    //feature = feature,
                                    x = cell.x,
                                    z = cell.z
                                });
                            }
                        }

                        num--;
                    }
                }
            }

            return true;  // It worked. TODO Make this actually reflect if it worked or not
        }


    }
}
