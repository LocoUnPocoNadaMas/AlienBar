using Utils;

namespace Model
{
    public class PlacedObject
    {
        public int ID { get; private set; }
        public int PlacedObjectIndex { get; private set; }
        public ObjectsType ObjectType { get; private set; }
        
        public PlacedObject(int id, int placedObjectIndex, ObjectsType objectType)
        {
            ID = id;
            PlacedObjectIndex = placedObjectIndex;
            ObjectType = objectType;
        }
    }
}