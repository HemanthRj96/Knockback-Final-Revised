using UnityEngine;
using System.Collections;

namespace Knockback.Core
{
    public class KB_PlayerKnockbackCore : MonoBehaviour
    {
        [Header("Knockback backend settings")]
        [Space]

        private int iterationPass = 10;
        private Rigidbody2D cachedRigidbody;
        private bool canUse = true;

        private void Awake()
        {
            if (cachedRigidbody == null)
                if (!TryGetComponent(out cachedRigidbody))
                    canUse = false;
        }

        /// <summary>
        /// Imparts knockback onto player controller
        /// </summary>
        /// <param name="knockbackAmount">Amount of knockback</param>
        /// <param name="direction">Direction of the knockback</param>
        public void CauseKnockback(float knockbackAmount, Vector2 direction)
        {
            if (!canUse)
                return;
            StartCoroutine(KnockbackPlayer(knockbackAmount, direction));
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