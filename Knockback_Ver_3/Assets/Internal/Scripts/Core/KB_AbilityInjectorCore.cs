using UnityEngine;
using Knockback.Controllers;
using Knockback.Helpers;

namespace Knockback.Core
{
    /// <summary>
    /// It should be inherited based on the type of injector or the type of ability it injects into player controller
    /// </summary>
    public class KB_AbilityInjectorCore : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Ability backend settings")]
        [SerializeField] private KB_MasterAbility targetAbility = new KB_MasterAbility();
        [SerializeField] private bool hasTriggerVolume = true;

        //** --PRIVATE ATTRIBUTES--

        private bool isPickedUp = false;
        private KB_PlayerController controller = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// This method is used to invoke the ability externally
        /// </summary>
        /// <param name="controller"></param>
        public void InvokeExternally(KB_PlayerController controller)
        {
            targetAbility.StartAbilityRoutine(controller, AbilityEndCallback);
        }

        //** --PROTECTED METHODS--

        /// <summary>
        /// Override this method to run a routine after the ability ends
        /// </summary>
        protected virtual void AbilityEndCallback() { }

        /// <summary>
        /// Override this method to run a routine after the ability begins
        /// </summary>
        protected virtual void AbilityBeginCallback() { }

        //** --PRIVATE METHODS--

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPickedUp || !hasTriggerVolume)
                return;
            if (collision.TryGetComponent(out controller))
            {
                isPickedUp = true;
                targetAbility.StartAbilityRoutine(controller, AbilityBeginCallback, AbilityEndCallback);
            }
        }
    }
}