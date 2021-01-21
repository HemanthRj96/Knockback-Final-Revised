using UnityEngine;
using Knockback.Scriptables;
using System.Collections.Generic;
using System;
using System.Collections;
using Knockback.Handlers;
using Knockback.Controllers;

namespace Knockback.Helpers
{
    [System.Serializable]
    public class KB_MasterAbility
    {

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private List<KB_AbilityBlueprint> abilities;
        [SerializeField] private float duration;

        //** --PRIVATE ATTIRBUTES--

        private KB_PlayerController controller;
        private Action abilityBeginFunctionCallback = null;
        private Action abilityEndFunctionCallback = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to activate the ability
        /// </summary>
        /// <param name="controller">Target player controller</param>
        /// <param name="abilityEndFunctionCallback">Optional callback function</param>
        public void StartAbilityRoutine(KB_PlayerController controller, Action abilityBeginFunctionCallback = null, Action abilityEndFunctionCallback = null)
        {
            this.controller = controller;
            this.abilityBeginFunctionCallback = abilityBeginFunctionCallback;
            this.abilityEndFunctionCallback = abilityEndFunctionCallback;
            KB_GameHandler.instance.StartCoroutine(AbilityRoutine());
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Routine which activates and deactivates the abilites
        /// </summary>
        private IEnumerator AbilityRoutine()
        {
            SetPlayerTarget();
            abilityBeginFunctionCallback?.Invoke();
            ActivateAbilities();
            yield return new WaitForSecondsRealtime(duration);
            DeactivateAbilities();
            abilityEndFunctionCallback?.Invoke();
        }

        /// <summary>
        /// Helper function to active all the abilities
        /// </summary>
        private void ActivateAbilities()
        {
            foreach(var ability in abilities)
                ability.ApplyAbility();
        }

        /// <summary>
        /// Helper function to deactivate all the abilities
        /// </summary>
        private void DeactivateAbilities()
        {
            foreach(var ability in abilities)
                ability.RemoveAbility();
        }

        /// <summary>
        /// Helper function to set the target player controller
        /// </summary>
        private void SetPlayerTarget()
        {
            foreach (var ability in abilities)
                ability.SetTargetPlayer(controller);
        }
    }
}