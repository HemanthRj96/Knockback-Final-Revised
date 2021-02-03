using UnityEngine;
using Knockback.Scriptables;
using System.Collections.Generic;
using System;
using System.Collections;
using Knockback.Handlers;
using Knockback.Controllers;

namespace Knockback.Helpers
{
    /// <summary>
    /// This is container class for abilities, it is used by abilityInjector classes for injecting the abilities 
    /// </summary>
    [System.Serializable]
    public class KB_MasterAbility
    {

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private List<KB_AbilityBlueprint> m_abilities;
        [SerializeField] private float m_duration;

        //** --PRIVATE ATTIRBUTES--

        private KB_PlayerController m_controller;
        private Action m_abilityBeginFunctionCallback = null;
        private Action m_abilityEndFunctionCallback = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to activate the ability
        /// </summary>
        /// <param name="controller">Target player controller</param>
        /// <param name="abilityEndFunctionCallback">Optional callback function</param>
        public void StartAbilityRoutine(KB_PlayerController controller, Action abilityBeginFunctionCallback = null, Action abilityEndFunctionCallback = null)
        {
            m_controller = controller;
            m_abilityBeginFunctionCallback = abilityBeginFunctionCallback;
            m_abilityEndFunctionCallback = abilityEndFunctionCallback;
            KB_GameHandler.instance.StartCoroutine(AbilityRoutine());
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Routine which activates and deactivates the abilites
        /// </summary>
        private IEnumerator AbilityRoutine()
        {
            SetPlayerTarget();
            m_abilityBeginFunctionCallback?.Invoke();
            ActivateAbilities();
            yield return new WaitForSecondsRealtime(m_duration);
            DeactivateAbilities();
            m_abilityEndFunctionCallback?.Invoke();
        }

        /// <summary>
        /// Helper function to active all the abilities
        /// </summary>
        private void ActivateAbilities()
        {
            foreach(var ability in m_abilities)
                ability.ApplyAbility();
        }

        /// <summary>
        /// Helper function to deactivate all the abilities
        /// </summary>
        private void DeactivateAbilities()
        {
            foreach(var ability in m_abilities)
                ability.RemoveAbility();
        }

        /// <summary>
        /// Helper function to set the target player controller
        /// </summary>
        private void SetPlayerTarget()
        {
            foreach (var ability in m_abilities)
                ability.SetTargetPlayer(m_controller);
        }
    }
}