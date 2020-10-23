using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System;

namespace Knockback.Handlers
{
    public class KB_PlayerProgressHandler : MonoBehaviour
    {
        //todo: Commenting

        internal class XPClass
        {
            public XPClass(int _level, float _currentXP)
            {
                this._level = _level;
                this._currentXP = _currentXP;
            }

            public XPClass() { }

            private float _currentXP = 0;
            private int _level = 1;

            private int UnitXP { get; } = 1000;
            private float XPIncrementer { get; } = 0.2f;
            public int MinXP { get; } = 0;
            public int MaxXP { get { return (int)(UnitXP + (UnitXP * XPIncrementer * (Level - 1))); } }
            public float CurrentXP { get { return _currentXP; } }
            public float AddXP { set { _currentXP += value; } }
            public int Level { get { return _level; } }
            public bool CheckXP { get { return _currentXP > MaxXP; } }

            public void LevelUp() => _level++;
            public void Reset() { _level = 1; _currentXP = 0; }
        }

        private static XPClass xpHandle = new XPClass();
        private KB_DatabaseHandler dataBase = new KB_DatabaseHandler();
        public string xpString { get; private set; }


        public void Start()
        {
            InitXPHandle();
        }

        public void AddXP(float xpAmount) => xpHandle.AddXP = xpAmount;

        public void LevelUp()
        {
            if (xpHandle.CheckXP)
            {
                xpHandle.LevelUp();
                OnLevelUp();
            }
        }

        public void OnLevelUp() => KB_EventHandler.Invoke("LEVELUP_EVENT");

        private void InitXPHandle()
        {
            int _level;
            float _currentXP;
            if (CheckDatabase())
            {
                xpString = dataBase.GetPlayerData().GetValue();
                ParseValueFromString(out _level, out _currentXP);
                xpHandle = new XPClass(_level, _currentXP);
            }
        }

        private bool CheckDatabase()
        {
            if (KB_DataPersistenceHandler.SaveExists(KB_DatabaseHandler.GetTargetDirectory()))
            {
                KB_DataPersistenceHandler.LoadData(KB_DatabaseHandler.GetTargetDirectory(), out dataBase);
                return true;
            }
            return false;
        }

        private void ParseValueFromString(out int _level, out float _currentXP)
        {
            _level = int.Parse(xpString.Split('|')[0]);
            _currentXP = float.Parse(xpString.Split('|')[1]);
        }

        private string CreateString() => $"{xpHandle.CurrentXP}|{xpHandle.Level}";
    }
}