using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public abstract class KB_AbilityContainerCore : ScriptableObject
    {
        public virtual void ActivateAbilityFromLocal(int containerId) { }

        public virtual void DeactivateAbilityFromLocal(int containerId) { }

        protected void AbilityActivator(_Ability _abilityContainer) => _abilityContainer.abilities.ForEach((ability) => ability.RemoveEffect());

        protected void AbilityDeactivator(_Ability _abilityContainer) => _abilityContainer.abilities.ForEach((ability) => ability.RemoveEffect());
    }

    [System.Serializable]
    public class _Ability
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