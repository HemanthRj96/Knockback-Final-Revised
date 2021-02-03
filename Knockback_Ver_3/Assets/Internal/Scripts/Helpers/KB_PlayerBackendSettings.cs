using UnityEngine;
using Knockback.Utility;

namespace Knockback.Helpers
{
    /// <summary>
    /// This is container class which holds readonly backend values for player, however it can be modified externally 
    /// by invoking the contructor
    /// </summary>
    [System.Serializable]
    public class KB_PlayerBackendSettings
    {
        //** --CONSTRUCTORS--

        public KB_PlayerBackendSettings() { }

        /// <summary>
        /// Invoke this constructor inorder to change the values of the backend variables
        /// </summary>
        /// <param name="variableType">Array of target backend variables we want to alter</param>
        /// <param name="values">Array of values for the target backend varaibles</param>
        public KB_PlayerBackendSettings(PlayerBackendSettingType[] variableType, dynamic[] values)
        {
            if (variableType.Length != values.Length)
                return;

            for (int index = 0; index < variableType.Length; index++)
            {
                switch (variableType[index])
                {
                    case PlayerBackendSettingType.moveSpeed:
                        m_moveSpeed = values[index];
                        break;
                    case PlayerBackendSettingType.jumpForce:
                        m_jumpForce = values[index];
                        break;
                    case PlayerBackendSettingType.airControl:
                        m_airControl = values[index];
                        break;
                    case PlayerBackendSettingType.groundCheckerLayerMask:
                        m_groundCheckerLayerMask = values[index];
                        break;
                    case PlayerBackendSettingType.dashingCooldown:
                        m_dashingCooldown = values[index];
                        break;
                    case PlayerBackendSettingType.dashingSpeed:
                        m_dashingSpeed = values[index];
                        break;
                    case PlayerBackendSettingType.dashingDistance:
                        m_dashingDistance = values[index];
                        break;
                    default:
                        break;
                }
            }
        }

        //** --ATTRIBUTES--
        //** --PUBLIC READONLY ATTRIBUTES--

        public readonly float m_moveSpeed = 10f;
        public readonly float m_jumpForce = 4f;
        public readonly float m_airControl = 0.65f;
        public LayerMask m_groundCheckerLayerMask = 1 << 10;
        public readonly float m_dashingCooldown = 0.85f;
        public readonly float m_dashingSpeed = 60;
        public readonly float m_dashingDistance = 4.5f;

        
    }
}