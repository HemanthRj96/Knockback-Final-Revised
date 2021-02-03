using UnityEngine;

namespace Knockback.Scriptables
{
    /// <summary>
    /// ScriptableObject for backend data for guns, changing parameters in this object creates variations of guns
    /// </summary>
    [CreateAssetMenu(fileName = "Gun Data", menuName = "Data/Gun data")]
    public class KB_GunBackendData : ScriptableObject
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Gun settings")]
        [Space]
        public string gunClass;
        public int gunId;
        public float bulletRoundsPerMinute;
        public float projectileSpread;
        public float projectileVelocity;
        public float cameraShakeIntensity;
        public float impactDamage;
        public float gunRecoil;
        public float reloadTime;
        public int roundCapacity;
        public int totalRounds;
    }
}


