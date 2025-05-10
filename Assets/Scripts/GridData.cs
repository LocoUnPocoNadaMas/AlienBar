using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Utils;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /// <summary>
    /// This method add an object to the grid if there are no previously occupied positions.
    /// </summary>
    /// <param name="gridPos">Starting position on the grid (dictionary key).</param>
    /// <param name="objectSize">3D dimensions of the object.</param>
    /// <param name="id">object.id</param>
    /// <param name="placedObjectIndex">Index of the placed object (increases with each placed object).</param>
    /// <param name="gridSize">Size of the grid area.</param>
    /// <param name="type"></param>
    /// <exception cref="Exception">Throws an exception if any cell is already occupied. Will it happen?</exception>
    public void AddObjectAt(Vector3Int gridPos, Vector3Int objectSize,
        int id, int placedObjectIndex, Vector2Int gridSize, ObjectsType type)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize);

        if (ArePositionsOccupied(positionToOccupy, gridSize, type) == -1)
            throw new Exception($"Dictionary already contains one of these cell positions");

        PlacementData data = new PlacementData(positionToOccupy, id, placedObjectIndex, type);
        // PlacedObject data = new(id, placedObjectIndex, type);
        foreach (var pos in positionToOccupy)
        {
            placedObjects[pos] = data;
        }
    }

    /// <summary>
    /// This method calculates all the positions on the grid that an object will occupy.
    /// </summary>
    /// <param name="gridPos">Starting position on the grid.</param>
    /// <param name="objectSize">3D dimensions of the object.</param>
    /// <returns>List of occupied positions</returns>
    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, Vector3Int objectSize)
    {
        List<Vector3Int> returnVector3Ints = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int z = 0; z < objectSize.z; z++)
            {
                for (int y = 0; y < objectSize.y; y++)
                {
                    returnVector3Ints.Add(gridPos + new Vector3Int(x, y, z));
                }
            }
        }

        return returnVector3Ints;
    }

    /// <summary>
    /// This method checks if an object can be placed on the grid without overlapping any other objects.
    /// </summary>
    /// <param name="gridPos">Starting position on the grid.</param>
    /// <param name="objectSize">3D dimensions of the object.</param>
    /// <param name="gridSize">Size of the grid area.</param>
    /// <param name="objectType"></param>
    /// <returns>True if the object can be placed, false if there is overlap.</returns>
    public int CanPlaceObjectAt(Vector3Int gridPos,
        Vector3Int objectSize, Vector2Int gridSize, ObjectsType objectType)
    {
        return ArePositionsOccupied(CalculatePositions(gridPos, objectSize), gridSize, objectType);
    }

    /// <summary>
    /// This method checks if any of the positions are occupied.
    /// </summary>
    /// <param name="positions">Positions to check.</param>
    /// <param name="gridSize">Size of the grid area.</param>
    /// <param name="objectType"></param>
    /// <returns>True if any position is occupied, false otherwise.</returns>
    private int ArePositionsOccupied(List<Vector3Int> positions, Vector2Int gridSize, ObjectsType objectType)
    {
        var halfGridX = gridSize.x / 2;
        var halfGridZ = gridSize.y / 2;
        var maxX = gridSize.x % 2 == 0 ? halfGridX : halfGridX + 1;
        var minX = halfGridX*-1;
        var maxZ = gridSize.y % 2 == 0 ? halfGridZ : halfGridZ + 1;
        var minZ = halfGridZ*-1;

        foreach (var pos in positions)
        {
            if (pos.x >= maxX || pos.x < minX)
                return -1;
            if (pos.z >= maxZ || pos.z < minZ)
                return -1;
            if (placedObjects.ContainsKey(pos))
            {
                // Debug.Log(pos);
                if (objectType == ObjectsType.Item)
                {
                    Debug.Log(objectType);
                    if (placedObjects[pos].ObjectType == ObjectsType.Furniture)
                    {
                        Debug.Log("on top");
                        var maxYPos = int.MinValue;
                        foreach (var yPos in placedObjects[pos].OccupiedPos)
                        {
                            if (yPos.y > maxYPos)
                            {
                                maxYPos = yPos.y;
                            }
                        }

                        return maxYPos;
                    }
                    else return -1;
                }

                return -1;
            }
        }
        return 0;
    }
}