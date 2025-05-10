using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Model
{
    public class PlacementData
    {
        public List<Vector3Int> OccupiedPos { get; private set; }
        public int ID { get; private set; }
        public int PlacedObjectIndex { get; private set; }
        public ObjectsType ObjectType { get; private set; }

        public PlacementData(List<Vector3Int> occupiedPos, int id, int placedObjectIndex, ObjectsType objectType)
        {
            OccupiedPos = occupiedPos;
            ID = id;
            PlacedObjectIndex = placedObjectIndex;
            ObjectType = objectType;
        }
    }
}