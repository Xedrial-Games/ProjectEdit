using ProjectEdit.Tiles;

namespace ProjectEdit
{
    public struct LevelData
    {
        public int ID;
        public string Name;
        public string Data;

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}";
        }
    }

    public struct Level
    {
        public int ID;
        public string Name;
        public TileInstance[] TilesData;

        public static readonly Level Empty = new(-1, string.Empty, null);

        public Level(int id, string name, TileInstance[] tilesInstances)
        {
            ID = id;
            Name = name;
            TilesData = tilesInstances;
        }
    }
}
