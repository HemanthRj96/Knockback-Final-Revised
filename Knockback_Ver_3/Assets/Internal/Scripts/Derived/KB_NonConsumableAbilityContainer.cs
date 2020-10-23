using Knockback.Handlers;
using Knockback.Utility;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;

namespace Knockback.Core
{
    [CreateAssetMenu(fileName = "KB_NonConsumableAbilityContainer", menuName = "Ability containers/Non-consumable container", order = 0)]
    public class KB_NonConsumableAbilityContainer : KB_AbilityContainerCore
    {
        [Header("Non consumable backend settings")]
        [Space]

        public List<_Ability> _nonConsumableAbilities = new List<_Ability>();
        private List<_Ability> _cachedActiveAbilities = new List<_Ability>();

        private void Awake()
        {
            KB_EventHandler.AddEvent("NCAbilityEvents", SampleEvent);
        }


        private void SampleEvent(IMessage message)
        {

        }

        /// <summary>
        /// Unlocks an ability with the container id populated inside the cache
        /// </summary>
        /// <param name="id">Id of the ability</param>
        /// <param name="source">Source injector</param>
        public void UnlockAbility(int id, KB_AbilityInjectorCore source = null)
        {
            int index;
            if (source == null || !KB_AbilityAuthenticatorCore.IsInjectorRegistered(source))
                return;
            if (!ContainsId(id, out index))
                return;
            _nonConsumableAbilities[index].Unlock();
        }

        /// <summary>
        /// Locks all abilities, not used under normal circumstances
        /// </summary>
        public void LockAllAbilities() => _nonConsumableAbilities.ForEach((ability) => ability.Lock());

        /// <summary>
        /// Locks an ability with the given index
        /// </summary>
        public void LockAbilityWithIndex(int index) => _nonConsumableAbilities[index].Lock();

        /// <summary>
        /// To activate ability from the cache
        /// </summary>
        /// <param name="id">Ability id</param>
        public override void ActivateAbilityFromLocal(int id)
        {
            int index;
            if (!ContainsId(id, out index))
                return;
            if (_nonConsumableAbilities[index].isUnlocked)
            {
                _nonConsumableAbilities[index].Activate();
                ActivateAbility(_nonConsumableAbilities[index]);
                _cachedActiveAbilities.Add(_nonConsumableAbilities[index]);
            }
        }

        /// <summary>
        /// To deactivate ability from the cache
        /// </summary>
        /// <param name="id">Ability id</param>
        public override void DeactivateAbilityFromLocal(int id)
        {
            int index;
            if (!ContainsId(id, out index))
                return;
            if(_nonConsumableAbilities[index].isActivated)
            {
                _nonConsumableAbilities[index].Deactivate();
                DeactivateAbility(_nonConsumableAbilities[index]);
                _cachedActiveAbilities.Remove(_nonConsumableAbilities[index]);
            }
        }

        /// <summary>
        /// Returns all active abilities
        /// </summary>
        public List<_Ability> GetAllActiveAbilities() => _cachedActiveAbilities;

        private void ActivateAbility(_Ability ability) => AbilityActivator(ability);
        
        private void DeactivateAbility(_Ability ability) => AbilityDeactivator(ability);

        private bool ContainsId(int id, out int index)
        {
            int tempIndex = 0;
            foreach (_Ability ability in _nonConsumableAbilities)
            {
                if (ability.id == id)
                {
                    index = tempIndex;
                    return true;
                }
                tempIndex++;
            }
            index = -1;
            return false;
        }

        /// <summary>
        /// Generates Id for abilities
        /// </summary>
        [ContextMenu("Generate Id")]
        private void GenerateAbilityId()
        {
            for (int i = 0; i < _nonConsumableAbilities.Count; i++)
            {
                _nonConsumableAbilities[i].SetId(this.GenerateId());
            }            
        }

        /// <summary>
        /// Return true if the Id exists
        /// </summary>
        private bool CheckIfIDExists(int id)
        {
            foreach (var ability in _nonConsumableAbilities)
                if (ability.id == id)
                    return true;
            return false;
        }

    }
}