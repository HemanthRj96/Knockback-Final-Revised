using UnityEngine;
using Knockback.Controllers;

namespace Knockback.Scriptables
{
    /// <summary>
    /// Should not use this class directly, instead create a new class which derives this class
    /// </summary>
    [System.Serializable]
    public class KB_AbilityBlueprint : ScriptableObject
    {

        //** --ATTRIBUTES--
        //** --PROTECTED ATTRIBUTES--

        protected KB_PlayerController controller = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to activate this effect
        /// </summary>
        public void ApplyAbility()
        {
            if (HasTarget())
                OnStartAbility();
        }

        /// <summary>
        /// Call this method to remove this effect
        /// </summary>
        public void RemoveAbility()
        {
            if (HasTarget())
                OnEndAbility();
        }

        /// <summary>
        /// Method to add set the target player controller
        /// </summary>
        /// <param name="controller">Target player controller</param>
        public void SetTargetPlayer(KB_PlayerController controller)
        {
            this.controller = controller;
        }

        //** --PROTECTED METHODS--

        /// <summary>
        /// Virtual method inside which you implement all the routine when the effect is in place
        /// </summary>
        protected virtual void OnStartAbility() { }

        /// <summary>
        /// Virtual method inside which you implement all the routine when the effect is removed
        /// </summary>
        protected virtual void OnEndAbility() { }

        //** --PRIVATE METHODDS--

        /// <summary>
        /// Returns true if there's a valid target onto which the effect can be  applied
        /// </summary>
        /// <returns></returns>
        private bool HasTarget() => controller != null;
    }
}