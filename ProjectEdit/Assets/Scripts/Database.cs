using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;

namespace ProjectEdit
{
    public static class Database
    {
        private static readonly HttpClient s_Client = new();
        private static readonly string s_ServerURI = "http://xedrialgames.atwebpages.com";
        //private static readonly string s_ServerURI = "http://localhost";

        public static async Task<List<Level>> GetLevels()
        {
            var response = await s_Client.GetAsync($"{s_ServerURI}/levels");
            string responseStr = await response.Content.ReadAsStringAsync();

            List<Level> levelsList = new();

            string[] levels = responseStr.Split(',');
            foreach (string level in levels)
            {
                Level lvl = new();

                string[] props = level.Split('<');
                foreach (string prop in props)
                {
                    string[] keyValue = prop.Split('>');

                    switch (keyValue[0])
                    {
                        case "0":
                            lvl.ID = Convert.ToInt32(keyValue[1]);
                            break;
                        case "1":
                            lvl.Name = keyValue[1];
                            break;
                        default:
                            break;
                    }
                }

                levelsList.Add(lvl);
            }

            return levelsList;
        }

        public async static Task<string> UploadLevel(string name, DirectoryInfo levelPath)
        {
            byte[] data = Serializer.CompressFolder(levelPath);

            Dictionary<string, string> post = new()
            {
                { "name", name },
                { "data", Convert.ToBase64String(data) }
            };

            var content = new FormUrlEncodedContent(post);

            var response = await s_Client.PostAsync($"{s_ServerURI}/levels/upload", content);
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public async static Task<string> GetLevel(int id)
        {
            var response = await s_Client.GetAsync($"{s_ServerURI}/levels/download/{id}");
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
