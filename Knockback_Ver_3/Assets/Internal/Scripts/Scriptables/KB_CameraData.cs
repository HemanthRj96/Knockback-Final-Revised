using UnityEngine;

namespace Knockback.Scriptables
{
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Data/Camera Data")]
    public class KB_CameraData : ScriptableObject
    {
        [Header("Camera controller backend settings")]
        [Space]

        public float cameraRestingZOffset;
        public float cameraFOV = 5;
        public Vector2 cameraDampingFactor;        
        public Vector2 offsetScaler;        
        public Vector2 offsetDampingFactor;        
        public Vector2 minimumBounds;        
        public Vector2 maximumBounds;

        [Header("Camera shake paramaters")]
        [Space]
        
        public float roughness;        
        public float fadeInTime;        
        public float fadeOutTime;
    }
}