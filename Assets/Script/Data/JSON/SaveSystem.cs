using System.IO;
using UnityEngine;

namespace Game.Data.Json
{
    public static class SaveSystem
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "savedata.json");

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }

        public static SaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                return new SaveData(); // chưa có save -> trả về data rỗng mặc định
            }

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public static bool HasSaveFile() => File.Exists(SavePath);
    }
}
