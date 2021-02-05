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
        [SerializeField] private KB_MasterAbility m_targetAbility = new KB_MasterAbility();
        [SerializeField] private bool m_hasTriggerVolume = true;

        //** --PRIVATE ATTRIBUTES--

        private bool m_isPickedUp = false;
        private KB_PlayerController m_controller = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// This method is used to invoke the ability externally
        /// </summary>
        /// <param name="controller"></param>
        public void InvokeExternally(KB_PlayerController controller)
        {
            m_targetAbility.StartAbilityRoutine(controller, AbilityEndCallback);
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
            if (m_isPickedUp || !m_hasTriggerVolume)
                return;
            if (collision.TryGetComponent(out m_controller))
            {
                m_isPickedUp = true;
                m_targetAbility.StartAbilityRoutine(m_controller, AbilityBeginCallback, AbilityEndCallback);
            }
        }
    }
}