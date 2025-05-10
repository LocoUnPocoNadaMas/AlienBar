using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    [CreateAssetMenu]
    public class ObjectsDatabaseSo : ScriptableObject
    {
        public List<ObjectData> objectsData;
        [SerializeField] private Vector2Int gridSize;

        public Vector2Int GetGridSize => gridSize;
    }
}


