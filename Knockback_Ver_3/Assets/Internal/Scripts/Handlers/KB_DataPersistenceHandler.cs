using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Knockback.Handlers
{
    /// <summary>
    /// Simple class to save and load file
    /// </summary>
    public static class KB_DataPersistenceHandler
    {
        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// To save the data to the target filePath
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="filePath">Name of the filePath with preceding forward slash</param>
        /// <param name="data">The data itself</param>
        public static void SaveData<T>(string filePath, T data) where T : class
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

        /// <summary>
        /// To load the data from the filePath
        /// </summary>
        /// <typeparam name="T">Type of the data type</typeparam>
        /// <param name="filePath">Name of the filePath</param>
        /// <param name="data">The data itself</param>
        public static void LoadData<T>(string filePath, out T data) where T : class
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

        /// <summary>
        /// Returns true if the file already exist in the filePath
        /// </summary>
        /// <param name="filePath">The filePath</param>
        /// <returns></returns>
        public static bool SaveExists(string filePath) => File.Exists(Application.persistentDataPath + filePath);

        //** --PRIVATE METHODS--

        /// <summary>
        /// Create a file
        /// </summary>
        /// <param name="filePath">Name of the file with a forward slash</param>
        private static void CreateFile(string filePath)
        {
            FileStream fileHandle = new FileStream(filePath, FileMode.Create);
            fileHandle.Close();
        }        
    }
}