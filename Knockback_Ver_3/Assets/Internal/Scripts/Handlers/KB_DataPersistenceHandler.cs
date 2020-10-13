using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Knockback.Handlers
{
    public static class KB_DataPersistenceHandler
    {
        //todo: Commenting :: Data persistence handler

        private static void CreateFile(string filePath)
        {
            string targetFilepath = Application.persistentDataPath + filePath;
            FileStream fileHandle = new FileStream(targetFilepath, FileMode.Create);
            fileHandle.Close();
        }

        public static void SaveData<T>(string filePath, T data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string targetFilepath = Application.persistentDataPath + filePath;

            // Create a file
            CreateFile(filePath);
            // Write data into the file
            FileStream fileHandle = new FileStream(targetFilepath, FileMode.Open);
            binaryFormatter.Serialize(fileHandle, data);
            // Close the file
            fileHandle.Close();
        }

        public static void LoadData<T>(string filePath, out T data)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            string targetFilepath = Application.persistentDataPath + filePath;

            if (SaveExists(filePath))
            {
                FileStream fileHandle = new FileStream(targetFilepath, FileMode.Open);
                data = (T)binaryFormatter.Deserialize(fileHandle);
                fileHandle.Close();
            }
            else { data = default(T); }
        }

        public static bool SaveExists(string filePath) { return File.Exists(Application.persistentDataPath + filePath); }
    }
}