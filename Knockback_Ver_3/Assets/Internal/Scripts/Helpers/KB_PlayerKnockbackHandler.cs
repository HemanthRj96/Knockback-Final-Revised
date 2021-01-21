using System.Collections;
using UnityEngine;
using Knockback.Controllers;

namespace Knockback.Helpers
{
    public class KB_PlayerKnockbackHandler
    {
        public KB_PlayerKnockbackHandler() { }
        public KB_PlayerKnockbackHandler(KB_PlayerController controlledActor, Rigidbody2D cachedRigidbody)
        {
            this.controlledActor = controlledActor;
            this.cachedRigidbody = cachedRigidbody;
        }

        [Header("Knockback backend settings")]
        [Space]

        private int iterationPass = 10;
        private bool canUse = true;
        private KB_PlayerController controlledActor = null;
        private Rigidbody2D cachedRigidbody;

        /// <summary>
        /// Imparts knockback onto player controller
        /// </summary>
        /// <param name="knockbackAmount">Amount of knockback</param>
        /// <param name="direction">Direction of the knockback</param>
        public void CauseKnockback(float knockbackAmount, Vector2 direction)
        {
            if (!canUse)
                return;
            controlledActor.StartCoroutine(KnockbackPlayer(knockbackAmount, direction));
        }

        private IEnumerator KnockbackPlayer(float recoilMagnitude, Vector2 direction)
        {
            int iteration = 0;
            while (iteration < 10)
            {
                cachedRigidbody.AddForce
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
