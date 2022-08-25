using ProjectEdit.Tiles;

namespace ProjectEdit
{
    public struct Level
    {
        public int ID;
        public string Name;
        public TileInstance[] TilesData;

        public static readonly Level Null = new(-1, string.Empty, null);

        private Level(int id, string name, TileInstance[] tilesInstances)
        {
            ID = id;
            Name = name;
            TilesData = tilesInstances;
        }
    }
}
