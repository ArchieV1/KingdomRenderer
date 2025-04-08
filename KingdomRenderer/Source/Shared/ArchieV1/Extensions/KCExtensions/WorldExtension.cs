using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static KingdomRenderer.Shared.ArchieV1.PrivateFieldTools;

namespace KingdomRenderer.Shared.ArchieV1.Extensions.KCExtensions
{
    /// <summary>
    /// Extension methods for World that add methods to access private fields.
    /// </summary>
    public static class WorldExtension
    {
        public static Cell[] GetCellData(this World world)
        {
            return (Cell[])PrivateFieldTools.GetPrivateField(world, "cellData");
        }

        /// <summary>
        /// Gets a private field from a given World instance with the given FieldName
        /// </summary>
        /// <param name="world"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldIsStatic"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if fieldName does not exist in the given context (Static/Instance)</exception>
        public static object GetPrivateField(this World world, string fieldName, bool fieldIsStatic = false)
        {
            return PrivateFieldTools.GetPrivateField(world, fieldName, fieldIsStatic);
        }

        /// <summary>
        /// Uses GetPrivateWorldField to get randomStoneState
        /// </summary>
        /// <param name="world">The instance of World randomStoneState will be read from</param>
        /// <returns>A copy of randomStoneState from the given world</returns>
        public static global::System.Random GetRandomStoneState(this World world)
        {
            return (global::System.Random)GetPrivateField(world, "randomStoneState");
        }

        /// <summary>
        /// Places a large stone feature on the given world.
        /// </summary>
        /// <param name="world">The world to place the large Stone Feature in.</param>
        /// <param name="cell">The central cell of the Stone Feature</param>
        /// <param name="primaryResourceType">The main ResourceType.</param>
        /// <param name="secondaryResourceType">The second ResourceType to surround the primary</param>
        /// <param name="chance1">Chance of growing the stoneGrowList</param>
        /// <param name="chance2">Chance of placing secondary resource after <see cref="chance1"/>. Chance of placing secondary resource is chance1 * chance2 per adjacent tile (4)y</param>

        /// <returns>TODO find out</returns>
        public static bool PlaceLargeStoneFeature(this World world, Cell cell, ResourceType primaryResourceType, ResourceType secondaryResourceType = ResourceType.UnusableStone, int chance1 = 60, int chance2 = 35)
        {
            bool flag = false;
            flag |= PlaceSmallStoneFeature(world, cell, primaryResourceType);
            if (!flag)
            {
                return false;
            }
            int num = SRand.Range(1, 2);
            for (int i = 0; i < num; i++)
            {
                int num2 = SRand.Range(2, 3);
                int num3 = (SRand.Range(0, 100) > 50) ? 1 : -1;
                int num4 = (SRand.Range(0, 100) > 50) ? 1 : -1;
                Cell cell2 = world.GetCellData(cell.x + num2 * num3, cell.z + num2 * num4);
                PlaceSmallStoneFeature(world, cell2, primaryResourceType, secondaryResourceType, chance1, chance2);
            }
            return flag;
        }

        /// <summary>
        /// Places a small stone featurein the given world.
        /// </summary>
        /// <param name="world">The world to place the large Stone Feature in.</param>
        /// <param name="cell">The central cell of the Stone Feature</param>
        /// <param name="primaryResourceType">The main ResourceType.</param>
        /// <param name="secondaryResourceType">The second ResourceType to surround the primary</param>
        /// <param name="chance1">Chance of growing the stoneGrowList</param>
        /// <param name="chance2">Chance of placing secondary resource after <see cref="chance1"/>. Chance of placing secondary resource is chance1 * chance2 per adjacent tile (4)y</param>
        /// <returns></returns>
        public static bool PlaceSmallStoneFeature(this World world, Cell cell, ResourceType primaryResourceType, ResourceType secondaryResourceType = ResourceType.UnusableStone, int chance1 = 60, int chance2 = 35)
        {
            Cell[] scratch4 = new Cell[4];
            world.GetNeighborCells(cell, ref scratch4);
            for (int i = 0; i < 4; i++)
            {
                if (scratch4[i] != null && scratch4[i].Type == ResourceType.Water)
                {
                    return false;
                }
            }

            // If first thing it does it clear the list it cant be that important? It is only used in this method and when it is defined with World.World() as so
            List<Cell> stoneGrowList = new List<Cell>();
            stoneGrowList.Clear();

            bool result = world.PlaceStone(cell, primaryResourceType);
            Vector3[] array = new Vector3[]
            {
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, 0f, 1f)
            };
            stoneGrowList.Add(cell);
            while (stoneGrowList.Count > 0)
            {
                int x = stoneGrowList[0].x;
                int z = stoneGrowList[0].z;
                stoneGrowList.RemoveAt(0);

                // If the zeroth position of stoneGrowList is the same as the Cell being passed num3=2 else num3=3
                // Num3 (+1) is the number of UnusableStone to place
                // First run (When placing around Resource) this WILL be true (3 stones)
                // Subsequent runs (When placing around an UnusableStone (21% chance) or adjacent to previous placement) will place up to 4 stones

                // End results:
                // In each direction:
                // 21% chance of placing UnusableStone + Adding that UnusableStoneCell to stoneGrowList
                // 60% chance add adding that UnusableStoneCell to stoneGrowList

                // Runs max 20 times cus of the `num -= 3;` 

                // This leads to the possible behaviour of a resource being placed and then nothing else for 20 tiles then 4 rocks around a point
                // The chance of this happening is:
                // ( {[3*0.40]*[1*(0.60*0.65)]} * {[3*0.43]*[1*(0.57*0.65)]} ... {[3*0.97]*[1*(0.03*0.65)]} ) * [4*0.03]
                // ( PROD_SUM (x=0.03, lim 0.6): [3*(0.40+x)]*[(0.60-x)*0.65] ) * [4*0.03]
                // ( PROD_SUM (x=1, lim 20): 1.95(0.40+x*0.03)(0.60-x*0.03) ) * [4*0.03]
                // = 1.29914E-10 * [4*0.04]
                // = 1.55879E-11
                // 1 in 100 000 000 000 
                // 1 in 100 billion chance
                // ....................U..
                // R..................U.U.
                // ....................U..
                int num3 = (x == cell.x && z == cell.z) ? 2 : 3;
                for (int j = 0; j < array.Length; j++)  // array.Length == 4
                {
                    if (SRand.Range(0, 100) < chance1)  // 60% chance
                    {
                        if (SRand.Range(0, 100) < chance2)  // 35% chance  ==> 21% chance of placing a stone. FOR EACH DIRECTION
                        {
                            Cell cell2 = world.GetCellData(x + (int)array[j].x, z + (int)array[j].z);
                            world.PlaceStone(cell2, secondaryResourceType);
                            num3--;
                        }
                        // This stops the for loop if placed all (2 : 3) stone.
                        // With how this is written it is not 2 or 3 stone it is 3 or 4 stones
                        if (num3 <= 0)
                        {
                            break;
                        }
                        // Add the unusable stone coords to the stone grow list
                        stoneGrowList.Add(world.GetCellData(x + (int)array[j].x, z + (int)array[j].z));
                    }
                }
                // Lowers the first % chance by 3%. Max 20 runs in the while loop
                chance1 -= 3;
            }
            return result;
        }

        /// <summary>
        /// Gets a copy of CellData from <paramref name="world"/>. Replaced position <paramref name="i"/> with <paramref name="cell"/>
        /// and saves back to <paramref name="world"/>.
        /// Use this if x and y position of <paramref name="cell"/> have not been set.
        /// </summary>
        /// <param name="world">The instance of world to update.</param>
        /// <param name="cell">The new cell information.</param>
        /// <param name="i">The position of the cell to replace inside of CellData.</param>
        public static void SetCellData(this World world, Cell cell, int i)
        {
            // Get a copy of cellData and edit it
            Cell[] newCellData = GetCellData(world);
            newCellData[i] = cell;

            SetPrivateField(world, "cellData", newCellData);
        }

        /// <summary>
        /// Updates the CellData of <paramref name="world"/> with <paramref name="cell"/>.
        /// </summary>
        /// <param name="world">The world to update.</param>
        /// <param name="cell">The cell to update.</param>
        public static void SetCellData(this World world, Cell cell)
        {
            int i = CellToCellDataIndex(world, cell);
            SetCellData(world, cell, i);
        }

        /// <summary>
        /// Replaces <paramref name="world"/>'s cellData with <paramref name="newCellData"/>.
        /// </summary>
        /// <param name="world">The world to update.</param>
        /// <param name="newCellData">The cellData to use for the replace.</param>
        public static void SetCellData(this World world, Cell[] newCellData)
        {
            SetPrivateField(world, "cellData", newCellData);
        }

        /// <summary>
        /// Converts <paramref name="cell"/>'s x and z coordinates to the index position of that cell in the 1D array of cells
        /// that is <paramref name="world"/>.CellData.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="cell"></param>
        /// <returns>The index position of <paramref name="cell"/> inside of <paramref name="world"/>.CellData.</returns>
        public static int CellToCellDataIndex(this World world, Cell cell)
        {
            return world.GridWidth * cell.Z + cell.x;
        }

        /// <summary>
        /// Converts cell <paramref name="index"/> to x or z coordinate using the other value.
        /// <paramref name="knownValue"/> is Z unless <paramref name="valueIsX"/> is true.
        /// </summary>
        /// <param name="world">The world which contains the cell data.</param>
        /// <param name="index">The index of the cell.</param>
        /// <param name="knownValue">The known value of the cell (Either X or Z). Assumed Z unless <paramref name="knownValue"/> is true.</param>
        /// <param name="valueIsX">Whether the known value is X or not.</param>
        /// <returns>Z if <paramref name="valueIsX"/> otherwise X.</returns>
        public static int CellDataIndexToCellCoords(this World world, int index, int knownValue, bool valueIsX = false)
        {
            if (valueIsX)
            {
                return (index - knownValue) / world.GridHeight; 
            }

            return index - knownValue * world.GridWidth;
        }

        /// <summary>
        /// Gets if there is any of the given ResourceType are within the given range of cell.
        /// </summary>
        /// <param name="world">The world to get the world data from.</param>
        /// <param name="cell">The cell to centre the search around.</param>
        /// <param name="resourceTypes">The resource types that are not allowed to be within <paramref name="range"/> of <paramref name="cell"/>.</param>
        /// <param name="range">The (square) range to search around <paramref name="cell"/>.</param>
        /// <returns>If any of the <paramref name="resourceTypes"/> are found.</returns>
        public static bool ResourceTypeWithinRange(this World world, Cell cell, IEnumerable<ResourceType> resourceTypes, int range)
        {
            var cellData = world.GetCellsData();
            var gridWidth = world.GridWidth;
            var gridHeight = world.GridHeight;

            int x = cell.x;
            int z = cell.z;

            bool foundResourceTypeCellWithinRange = false;
            int xPosLower = Mathff.Clamp(x - range, 0, gridWidth - 1);
            int zPosLower = Mathff.Clamp(z - range, 0, gridHeight - 1);
            int xPosUpper = Mathff.Clamp(x + range, 0, gridWidth - 1);
            int zPosUpper = Mathff.Clamp(z + range, 0, gridHeight - 1);
            // Will iterate through:
            // [zPos-range, ..., zPos-1, zPos, zPos+1, ..., zPos+range]
            // Clamping means it will be same as above where it could be less than 5 values if some values go off of the map edge
            for (int zPos = zPosLower; zPos <= zPosUpper; zPos++)
            {
                for (int xPos = xPosLower; xPos <= xPosUpper; xPos++)
                {
                    Cell searchedCell = cellData[zPos * gridWidth + xPos];
                    if (resourceTypes.Contains(searchedCell.Type))
                    {
                        foundResourceTypeCellWithinRange = true;
                    }
                }
            }

            return foundResourceTypeCellWithinRange;
        }

        /// <summary>
        /// Counts the number of cells with trees within <paramref name="range"/> of <paramref name="cell"/>.
        /// </summary>
        /// <param name="world">The world to get the world data from.</param>
        /// <param name="cell">The cell to centre the search around.</param>
        /// <param name="range">The (square) range to search around <paramref name="cell"/>.</param>
        /// <returns>The number of trees found.</returns>
        public static int TreeCellsWithinRange(this World world, Cell cell, int range)
        {
            int gridHeight = world.GridHeight;
            int gridWidth = world.GridWidth;
            int x = cell.x;
            int z = cell.z;
            Cell[] cellData = world.GetCellData();

            int numSurroundingCellsWithTrees = 0;
            // Makes sure x and z (+-1) are between 0 and gridwidth/gridheight
            // Makes sure (+-) x, z are inside the map
            int xPosLower = Mathff.Clamp(x - range, 0, gridWidth - 1);
            int zPosLower = Mathff.Clamp(z - range, 0, gridHeight - 1);
            int xPostUpper = Mathff.Clamp(x + range, 0, gridWidth - 1);
            int zPosUpper = Mathff.Clamp(z + range, 0, gridHeight - 1);

            // The clamping means that loop will be any of:
            // [cellZ-1, cellZ, cellZ+1]
            // [cellZ, cellZ+1]
            // [cellZ-1, cellZ]
            // Theoretical [cellZ] but that would need a 1x1 size map
            for (int zPos = zPosLower; zPos <= zPosUpper; zPos++)
            {
                // For [cellZ-1, cellZ, cellZ+1]
                // Clamped though so cellZ-1 and cellZ+1 will be valid for sure
                for (int xPos = xPosLower; xPos <= xPostUpper; xPos++)
                {
                    // For [cellX-1, cellX, cellX+1]
                    // Clamped though so cellX-1 and cellX+1 will be valid for sure
                    if (cellData[zPos * gridWidth + xPos].TreeAmount > 0)
                    {
                        numSurroundingCellsWithTrees++;
                    }
                }
            }

            return numSurroundingCellsWithTrees;
        }

        /// <summary>
        /// Returns if there are too many cells with trees surround <paramref name="cell"/> in a range of <paramref name="range"/>.
        /// </summary>
        /// <param name="world">The world to get the world data from.</param>
        /// <param name="cell">The cell to centre the search around.</param>
        /// <param name="maxNumberTrees">The maximum number of trees that can be found.</param>
        /// <param name="range">The (square) range to search around <paramref name="cell"/>.</param>
        /// <returns>True if more cells with trees than <paramref name="maxNumberTrees"/> are found.</returns>
        public static bool TooManyTreeCellsWithinRange(this World world, Cell cell, int maxNumberTrees, int range)
        {
            return TreeCellsWithinRange(world, cell, range) > maxNumberTrees;
        }
    }
}