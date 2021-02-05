using UnityEngine;
using System.Collections.Generic;

namespace Knockback.Handlers
{
    /// <summary>
    /// Database handlers are used to save and load data offline if necessary and more data type can be added
    /// </summary>
    public class KB_DatabaseHandler
    {
        //** --ATTRIBUTES--
        //** ---PRIVATE ATTRIBUTES--

        private static readonly string TARGET_DIRECTORY = "/KnockbackDataBase.knockback";

        private PlayerSaveData m_playerSaveData = new PlayerSaveData();
        private JoystickSaveData m_joystickSaveData = new JoystickSaveData();
        private GameStateData m_gameStateData = new GameStateData();

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// All getter methods for the data
        /// </summary>
        public ref PlayerSaveData GetPlayerData() { return ref m_playerSaveData; }
        public ref JoystickSaveData GetJoystickData() { return ref m_joystickSaveData; }
        public ref GameStateData GetGameStateData() { return ref m_gameStateData; }
        public static string GetTargetDirectory() { return TARGET_DIRECTORY; }
    }

    /// <summary>
    /// This data structure saves the player save data
    /// </summary>
    [System.Serializable]
    public struct PlayerSaveData
    {
        private string value;
        public void SetValue(string value) => this.value = value;
        public string GetValue() => value;

    }

    /// <summary>
    /// This data structure saves the joystick data
    /// </summary>
    [System.Serializable]
    public struct JoystickSaveData
    {
        private List<float[]> targetTransform;

        public void CopyToJoystickData(Dictionary<int, Transform> UIButtonCollection)
        {
            List<float[]> tempList = new List<float[]>();

            foreach (var buttons in UIButtonCollection)
            {
                tempList.Add(new float[4] { buttons.Value.position.x, buttons.Value.position.y, buttons.Value.position.z, buttons.Value.localScale.x });
            }
            targetTransform = tempList;
        }

        public void CopyFromJoystickData(Dictionary<int, Transform> UIButtonCollection)
        {
            int index = 0;
            foreach (var transform in targetTransform)
            {
                UIButtonCollection[index].position = new Vector3(transform[0], transform[1], transform[2]);
                UIButtonCollection[index].localScale = new Vector3(transform[3], transform[3], 0);
                index++;
            }
        }
    }

    /// <summary>
    /// Data structure to save game state data
    /// </summary>
    [System.Serializable]
    public struct GameStateData
    {
        private bool firstRun;

        public bool CheckFirstRun() { return firstRun; }
        public void SetFirstRun() { firstRun = true; }
    }
}