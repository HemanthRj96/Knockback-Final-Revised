using Knockback.Helpers;
using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public abstract class KB_AbilityContainerCore : ScriptableObject
    {
        public virtual void ActivateAbilityFromLocal(int containerId) { }
        public virtual void DeactivateAbilityFromLocal(int containerId) { }
        protected void AbilityActivator(KB_Ability _abilityContainer) => _abilityContainer.abilities.ForEach((ability) => ability.RemoveEffect());
        protected void AbilityDeactivator(KB_Ability _abilityContainer) => _abilityContainer.abilities.ForEach((ability) => ability.RemoveEffect());
    }    
}