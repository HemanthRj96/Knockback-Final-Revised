using System.Collections;
using UnityEngine;
using Knockback.Controllers;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class implements the knockback mechanism
    /// </summary>
    public class KB_PlayerKnockbackHandler
    {
        //** --CONSTRUCTOR--

        public KB_PlayerKnockbackHandler() { }
        public KB_PlayerKnockbackHandler(KB_PlayerController controlledActor, Rigidbody2D cachedRigidbody)
        {
            this.m_controlledActor = controlledActor;
            this.m_cachedRigidbody = cachedRigidbody;
        }

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Knockback backend settings")]
        [Space]
        [SerializeField] private int m_iterationPass = 10;

        //** --PRIVATE ATTIBUTES--

        private bool m_canUse = true;
        private KB_PlayerController m_controlledActor = null;
        private Rigidbody2D m_cachedRigidbody;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Imparts knockback onto player controller
        /// </summary>
        /// <param name="knockbackAmount">Amount of knockback</param>
        /// <param name="direction">Direction of the knockback</param>
        public void CauseKnockback(float knockbackAmount, Vector2 direction)
        {
            if (!m_canUse)
                return;
            m_controlledActor.StartCoroutine(KnockbackPlayer(knockbackAmount, direction));
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Coroutine that knocks back player towards a specific direction wtih a magnitude
        /// </summary>
        /// <param name="recoilMagnitude"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private IEnumerator KnockbackPlayer(float recoilMagnitude, Vector2 direction)
        {
            int iteration = 0;
            while (iteration < 10)
            {
                m_cachedRigidbody.AddForce
                    (
                        direction * ((recoilMagnitude * (10 - iteration) / 10) / 50),
                        ForceMode2D.Impulse
                    );
                ++iteration;
                yield return new WaitForFixedUpdate();
            }
            yield return null;
        }
    }
}
