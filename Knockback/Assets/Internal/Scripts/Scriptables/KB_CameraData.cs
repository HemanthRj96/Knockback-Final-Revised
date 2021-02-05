using UnityEngine;

namespace Knockback.Scriptables
{
    /// <summary>
    /// ScriptableObject for the camera data and parameters
    /// </summary>
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Data/Camera Data")]
    public class KB_CameraData : ScriptableObject
    {
        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

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