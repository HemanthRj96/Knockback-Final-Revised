using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_AbilityContainerCore : ScriptableObject
    {
        public virtual void ActivateContainerAbility(int containerId, bool autoDeactivate = true) { }

        public virtual void ActivateContainerAbility(string containerTag, bool autoDeactivate = true) { }

        public virtual void DeactivateContainerAbility(int containerId) { }

        public virtual void DeactivateContainerAbility(string containerTag) { }


        protected void AbilityActivater(AbilityContainer container)
        {
            foreach (KB_AbilityCore ability in container.unitAbilities)
            {
                ability.ApplyEffect();
            }
        }

        protected void AbilityDeactivator(AbilityContainer container)
        {
            foreach (KB_AbilityCore ability in container.unitAbilities)
            {
                ability.RemoveEffect();
            }
        }


    }

    [System.Serializable]
    public struct AbilityContainer
    {
        public int containerId;
        public string containerTag;
        public List<KB_AbilityCore> unitAbilities;        
        public float abilityDuration;
        public float abilityCooldown;

        public AbilityContainer(int containerId, string containerTag, List<KB_AbilityCore> unitAbilities, float abilityDuration = 5, float abilityCooldown = 25)
        {
            this.containerId = containerId;
            this.containerTag = containerTag;
            this.unitAbilities = unitAbilities;
            this.abilityDuration = abilityDuration;
            this.abilityCooldown = abilityCooldown;
        }
    }

}