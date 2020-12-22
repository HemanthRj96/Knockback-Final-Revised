using UnityEngine;
using Knockback.Core;
using System.Collections.Generic;

namespace Knockback.Helpers
{
    [System.Serializable]
    public class KB_Ability
    {
        [HideInInspector]
        public bool isUnlocked;
        [HideInInspector]
        public bool isActivated;
        //[HideInInspector]
        public int id;
        public List<KB_AbilityCore> abilities;
        public float duration;
        public float cooldown;

        public void SetId(int id)
        {
            if (this.id == 0)
                this.id = id;
        }
        public void Unlock() => isUnlocked = true;
        public void Lock() => isUnlocked = false;
        public void Activate() => isActivated = true;
        public void Deactivate() => isActivated = false;

    }
}