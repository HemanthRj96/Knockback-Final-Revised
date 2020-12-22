﻿using UnityEngine;
using Knockback.Core;
using System.Collections.Generic;
using Knockback.Utility;
using Knockback.Handlers;
using Knockback.Helpers;

namespace Knockback.Derived
{
    public class KB_ConsumableAbilityContainer : KB_AbilityContainerCore
    {

        [Header("Non consumable backend settings")]
        [Space]

        public List<KB_Ability> _consumableAbilities = new List<KB_Ability>();
        private List<KB_Ability> _cachedActiveAbilities = new List<KB_Ability>();

        private void Awake()
        {
            KB_EventHandler.AddEvent("CAbilityEvents", SampleEvent);
        }


        private void  SampleEvent(IMessage message)
        {

        }

        public void AddConsumableAbility(int id, KB_AbilityInjectorCore source = null)
        {

        }

        /// <summary>
        /// To activate ability from the cache
        /// </summary>
        /// <param name="id">Ability id</param>
        public override void ActivateAbilityFromLocal(int id)
        {
            int index;
            if (!ContainsId(id, out index))
                return;
            if (_consumableAbilities[index].isUnlocked)
            {
                _consumableAbilities[index].Activate();
                ActivateAbility(_consumableAbilities[index]);
                _cachedActiveAbilities.Add(_consumableAbilities[index]);
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
            if (_consumableAbilities[index].isActivated)
            {
                _consumableAbilities[index].Deactivate();
                DeactivateAbility(_consumableAbilities[index]);
                _cachedActiveAbilities.Remove(_consumableAbilities[index]);
            }
        }

        /// <summary>
        /// Returns all active abilities
        /// </summary>
        public List<KB_Ability> GetAllActiveAbilities() => _cachedActiveAbilities;

        private void ActivateAbility(KB_Ability ability) => AbilityActivator(ability);

        private void DeactivateAbility(KB_Ability ability) => AbilityDeactivator(ability);

        private bool ContainsId(int id, out int index)
        {
            int tempIndex = 0;
            foreach (KB_Ability ability in _consumableAbilities)
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
            for (int i = 0; i < _consumableAbilities.Count; i++)
            {
                _consumableAbilities[i].SetId(this.GenerateId());
            }
        }

        /// <summary>
        /// Return true if the Id exists
        /// </summary>
        private bool CheckIfIDExists(int id)
        {
            foreach (var ability in _consumableAbilities)
                if (ability.id == id)
                    return true;
            return false;
        }
    }
}