using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Knockback.Core
{
    [CreateAssetMenu(fileName = "KB_NonConsumableAbilityContainer", menuName = "Ability containers/Non-consumable container", order = 0)]
    public class KB_NonConsumableAbilityContainer : KB_AbilityContainerCore
    {
        [Header("Non consumable backend settings")]
        [Space]
        // These are abilities that cannot be consumed these includes abilites that come by default with a player charactera and abilities unlocked with levels
        //tier-1 : Not as rare as tier-2
        [SerializeField]
        [Tooltip("Do not change values during runtime as it resets all colllections")]
        private List<AbilityContainer> nonconsumableContainerAbilities = new List<AbilityContainer>();
        private Dictionary<AbilityContainer, bool> containerLookupCollection = new Dictionary<AbilityContainer, bool>();
        private List<AbilityContainer> cachedActiveAbilities = new List<AbilityContainer>();

        public void UnlockAbility(int containerId, KB_AbilityInjectorCore source = null)
        {
            if (source == null || !KB_AbilityAuthenticatorCore.IsInjectorRegistered(source))
                return;
            
            int index;
            if (!ContainsId(containerId, out index))
                return;
            
        }

        private bool ContainsId(int containerId, out int index)
        {
            int tempIndex = 0;
            foreach(AbilityContainer container in nonconsumableContainerAbilities)
            {
                if (container.containerId == containerId)
                {
                    index = tempIndex; 
                    return true;
                }
                tempIndex++;
            }
            index = -1;
            return false;
        }

        private void OnValidate()
        {
            InitCollections();
        }

        private void InitCollections()
        {
            containerLookupCollection.Clear();
            nonconsumableContainerAbilities.ForEach((container) => containerLookupCollection.Add(container,false));
        }

    }
}