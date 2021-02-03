using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System;

namespace Knockback.Handlers
{
    //todo: Refactoring - KB_PlayerProgressHandler

    /// <summary>
    /// This method handles all the player progress
    /// </summary>
    public class KB_PlayerProgressHandler : MonoBehaviour
    {
        //** --INTERNAL CLASS--

        internal class XPClass
        {
            public XPClass(int _level, float _currentXP)
            {
                this.mi_level = _level;
                this.mi_currentXP = _currentXP;
            }

            public XPClass() { }

            private float mi_currentXP = 0;
            private int mi_level = 1;

            private int unitXP { get; } = 1000;
            private float xPIncrementer { get; } = 0.2f;
            public int minXP { get; } = 0;
            public int maxXP { get { return (int)(unitXP + (unitXP * xPIncrementer * (level - 1))); } }
            public float currentXP { get { return mi_currentXP; } }
            public float addXP { set { mi_currentXP += value; } }
            public int level { get { return mi_level; } }
            public bool checkXP { get { return mi_currentXP > maxXP; } }

            public void LevelUp() => mi_level++;
            public void Reset() { mi_level = 1; mi_currentXP = 0; }
        }

        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES

        private static XPClass m_xpHandle = new XPClass();
        private KB_DatabaseHandler m_dataBase = new KB_DatabaseHandler();

        //** --PUBLIC REFERENCE--

        public string m_xpString { get; private set; }

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to add XP to the progress amount
        /// </summary>
        /// <param name="xpAmount"></param>
        public void AddXP(float xpAmount) => m_xpHandle.addXP = xpAmount;

        //** --PRIVATE METHODS--

        /// <summary>
        /// Do this onStart
        /// </summary>
        private void Start() => Bootstrapper();

        /// <summary>
        /// Method initializes the dafult values
        /// </summary>
        private void Bootstrapper()
        {
            int _level;
            float _currentXP;
            if (CheckDatabase())
            {
                m_xpString = m_dataBase.GetPlayerData().GetValue();
                ParseValueFromString(out _level, out _currentXP);
                m_xpHandle = new XPClass(_level, _currentXP);
            }
        }

        /// <summary>
        /// Returns true if a database exists and false otherwise
        /// </summary>
        private bool CheckDatabase()
        {
            if (KB_DataPersistenceHandler.SaveExists(KB_DatabaseHandler.GetTargetDirectory()))
            {
                KB_DataPersistenceHandler.LoadData(KB_DatabaseHandler.GetTargetDirectory(), out m_dataBase);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to parse values from strings
        /// </summary>
        /// <param name="_level"></param>
        /// <param name="_currentXP"></param>
        private void ParseValueFromString(out int _level, out float _currentXP)
        {
            _level = int.Parse(m_xpString.Split('|')[0]);
            _currentXP = float.Parse(m_xpString.Split('|')[1]);
        }

        /// <summary>
        /// Method to encode level and xp into string
        /// </summary>
        private string CreateString() => $"{m_xpHandle.currentXP}|{m_xpHandle.level}";

        /// <summary>
        /// Do this on level up
        /// </summary>
        private void LevelUp()
        {
            if (m_xpHandle.checkXP)
            {
                m_xpHandle.LevelUp();
                OnLevelUp();
            }
        }

        /// <summary>
        /// Call event on level up
        /// </summary>
        private void OnLevelUp() => KB_EventHandler.Invoke("LEVELUP_EVENT");
    }
}