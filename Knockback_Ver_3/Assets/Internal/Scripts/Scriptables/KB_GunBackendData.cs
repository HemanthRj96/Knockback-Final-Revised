using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Scriptables
{
    [CreateAssetMenu(fileName = "Gun Data", menuName = "Data/Gun data")]
    public class KB_GunBackendData : ScriptableObject
    {
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


