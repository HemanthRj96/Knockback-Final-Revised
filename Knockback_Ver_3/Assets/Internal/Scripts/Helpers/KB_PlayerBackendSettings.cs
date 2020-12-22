using UnityEngine;

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

        public KB_PlayerBackendSettings(int[] variableModifierIndex, dynamic[] values)
        {
            if (variableModifierIndex.Length != values.Length)
                return;

            for (int index = 0; index < variableModifierIndex.Length; index++)
            {
                switch (variableModifierIndex[index])
                {
                    case 0:
                        moveSpeed = values[index];
                        break;
                    case 1:
                        jumpForce = values[index];
                        break;
                    case 2:
                        airControl = values[index];
                        break;
                    case 3:
                        groundCheckerLayerMask = values[index];
                        break;
                    case 4:
                        dashingCooldown = values[index];
                        break;
                    case 5:
                        dashingSpeed = values[index];
                        break;
                    case 6:
                        dashingDistance = values[index];
                        break;
                    default:
                        break;
                }
            }
        }
    }
}