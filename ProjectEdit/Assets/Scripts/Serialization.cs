using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ProjectEdit.Tiles;

namespace ProjectEdit
{
    public class Serializer
    {
        public static DirectoryInfo LevelsPath => new($"{Application.persistentDataPath}/Levels");
        public static DirectoryInfo OnlineLevelsPath => new($"{Application.persistentDataPath}/OnlineLevels");

        public DirectoryInfo[] LevelsPaths => m_RootDirectory.GetDirectories();

        public event Action OnUpdateLevel;

        public List<Level> Levels { get; private set; }

        private readonly List<int> m_LevelsIDs;
        private readonly DirectoryInfo m_RootDirectory;

        public Serializer(DirectoryInfo rootDirectory)
        {
            m_RootDirectory = rootDirectory;

            if (!m_RootDirectory.Exists)
                m_RootDirectory.Create();

            Levels = new();
            m_LevelsIDs = new();

            UpdateLevels();
        }

        public void DeleteLevel(Level level)
        {
            Levels.Remove(level);
            m_LevelsIDs.Remove(level.ID);

            Directory.Delete($"{m_RootDirectory}/{level.ID}", true);

            UpdateLevels();
        }

        public void UpdateLevels()
        {
            Levels.Clear();
            m_LevelsIDs.Clear();

            foreach (DirectoryInfo levelDirectory in LevelsPaths)
            {
                FileInfo levelFile = levelDirectory.GetFiles()[0];

                Level level;
                if (levelFile.Extension == ".lvl")
                    level = DeserializeLevel(levelFile.FullName);
                else if (levelFile.Extension == ".clvl")
                    level = ExtractLevel(Deserialize(levelFile.FullName));
                else
                {
                    Debug.LogError($"No level found on path '{levelDirectory.FullName}'");
                    return;
                }

                Levels.Add(level);
                m_LevelsIDs.Add(level.ID);
            }

            OnUpdateLevel?.Invoke();
        }

        public DirectoryInfo GetLevelPath(Level level) =>
            new($"{m_RootDirectory.FullName}/{level.ID}");

        public async Task<Level> GetOnlineLevel(int id)
        {
            if (!m_LevelsIDs.Contains(id))
            {
                string responseString = await Database.GetLevel(id);

                Level level = ExtractLevel(responseString);

                DirectoryInfo directoryInfo = Directory.CreateDirectory($"{m_RootDirectory}/{level.ID}");
                File.WriteAllText($"{directoryInfo.FullName}/{level.Name}.clvl", responseString);

                UpdateLevels();
            }

            return Levels.Find(a => a.ID == id);
        }

        public void Serialize(string path, string data) => File.WriteAllText(path, data);

        public void SerializeLevel(Level level)
        {
            if (!m_RootDirectory.Exists)
                m_RootDirectory.Create();

            ISerializer serializer = new SerializerBuilder()
                .WithTypeConverter(new VectorYamlTypeConverter())
                .Build();

            string path;

            if (!m_LevelsIDs.Contains(level.ID))
                path = CreateNewLevel(out level.ID);
            else path = new($"{m_RootDirectory}/{level.ID}");

            string yaml = serializer.Serialize(level);

            Serialize($"{path}/{level.Name}.lvl", yaml);

            UpdateLevels();
        }

        private string CreateNewLevel(out int id)
        {
            string path = string.Empty;
            id = 0;

            for (int i = 0; i < LevelsPaths.Length; i++)
            {
                if (LevelsPaths[i].Name != i.ToString())
                {
                    path = Directory.CreateDirectory($"{m_RootDirectory}/{i}").FullName;
                    id = i;
                }
            }

            if (path == string.Empty)
            {
                path = Directory.CreateDirectory($"{m_RootDirectory}/{LevelsPaths.Length}").FullName;
                id = LevelsPaths.Length - 1;
            }

            return path;
        }

        public string Deserialize(string path) => File.ReadAllText(path);

        public Level DeserializeLevel(string path)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithTypeConverter(new VectorYamlTypeConverter())
                .Build();

            string levelYaml = Deserialize(path);
            Level level = deserializer.Deserialize<Level>(levelYaml);

            return level;
        }

        public string SerializeTilesDataCompressed(TileInstance[] tilesData)
        {
            string compressedFile = string.Empty;

            foreach (TileInstance tileData in tilesData)
            {
                compressedFile += $"0:{tileData.CellPosition.x},{tileData.CellPosition.y}+";
                compressedFile += $"1:{tileData.Index:X};";
            }

            compressedFile = compressedFile[..^1];

            return compressedFile;
        }

        public static Level ExtractLevel(string levelData)
        {
            if (levelData == "null")
                return Level.Empty;

            Level level = new();

            string[] data = levelData.Split('<');
            foreach (string prop in data)
            {
                string[] keyValue = prop.Split('>');
                switch (keyValue[0])
                {
                    case "0":
                        level.ID = Convert.ToInt32(keyValue[1]);
                        break;
                    case "1":
                        level.Name = keyValue[1];
                        break;
                    case "2":
                        level.TilesData = ExtractTiles(keyValue[1]);
                        break;
                    default:
                        break;
                }
            }

            return level;
        }

        public static TileInstance[] ExtractTiles(string tilesData)
        {
            List<TileInstance> compressedTilesData = new();
            string[] tiles = tilesData.Split(';');

            foreach (string tile in tiles)
            {
                TileInstance tileData = new();

                string[] properties = tile.Split('+');
                foreach (string property in properties)
                {
                    string[] keyValue = property.Split(':');
                    switch (keyValue[0])
                    {
                        case "0":
                            string[] position = keyValue[1].Split(',');
                            tileData.CellPosition.x = Convert.ToInt32(position[0]);
                            tileData.CellPosition.y = Convert.ToInt32(position[1]);
                            break;
                        case "1":
                            tileData.Index = Convert.ToInt32(keyValue[1], 16);
                            break;
                        default:
                            break;
                    }
                }

                compressedTilesData.Add(tileData);
            }

            return compressedTilesData.ToArray();
        }

        public static byte[] CompressFolder(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
                return null;

            FileInfo[] filesToCmprs = directoryInfo.GetFiles();
            MemoryStream baseStream = new();

            using (Stream outputStream = new GZipOutputStream(baseStream))
            {
                using TarArchive tarArchive = TarArchive.CreateOutputTarArchive(outputStream);
                tarArchive.RootPath = directoryInfo.FullName;
                foreach (var fi in filesToCmprs)
                {
                    TarEntry entry = TarEntry.CreateEntryFromFile(fi.FullName);
                    tarArchive.WriteEntry(entry, true);
                }
            }

            byte[] result = baseStream.ToArray();
            baseStream.Dispose();
            return result;
        }

        public DirectoryInfo ExtractData(byte[] data)
        {
            DirectoryInfo targetDirectory = new($"{m_RootDirectory.FullName}");
            if (!targetDirectory.Exists)
                targetDirectory.Create();

            using MemoryStream baseStream = new(data, false);
            using Stream sourceStream = new GZipInputStream(baseStream);
            using TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, 
                TarBuffer.DefaultBlockFactor, System.Text.Encoding.Default);

            tarArchive.ExtractContents(targetDirectory.FullName);

            return targetDirectory;
        }
    }
}
