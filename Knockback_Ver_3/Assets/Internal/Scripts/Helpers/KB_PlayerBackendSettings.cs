using UnityEngine;
using Knockback.Utility;

namespace Knockback.Helpers
{
    [System.Serializable]
    public class KB_PlayerBackendSettings
    {

        public readonly float moveSpeed = 10f;
        public readonly float jumpForce = 4f;
        public readonly float airControl = 0.65f;
        public LayerMask groundCheckerLayerMask = 1 << 10;
        public readonly float dashingCooldown = 0.85f;
        public readonly float dashingSpeed = 60;
        public readonly float dashingDistance = 4.5f;

        public KB_PlayerBackendSettings() { }

        public KB_PlayerBackendSettings(PlayerBackendSettingType[] variableType, dynamic[] values)
        {
            if (variableType.Length != values.Length)
                return;

            for (int index = 0; index < variableType.Length; index++)
            {
                switch (variableType[index])
                {
                    case PlayerBackendSettingType.moveSpeed:
                        moveSpeed = values[index];
                        break;
                    case PlayerBackendSettingType.jumpForce:
                        jumpForce = values[index];
                        break;
                    case PlayerBackendSettingType.airControl:
                        airControl = values[index];
                        break;
                    case PlayerBackendSettingType.groundCheckerLayerMask:
                        groundCheckerLayerMask = values[index];
                        break;
                    case PlayerBackendSettingType.dashingCooldown:
                        dashingCooldown = values[index];
                        break;
                    case PlayerBackendSettingType.dashingSpeed:
                        dashingSpeed = values[index];
                        break;
                    case PlayerBackendSettingType.dashingDistance:
                        dashingDistance = values[index];
                        break;
                    default:
                        break;
                }
            }
        }
    }
}