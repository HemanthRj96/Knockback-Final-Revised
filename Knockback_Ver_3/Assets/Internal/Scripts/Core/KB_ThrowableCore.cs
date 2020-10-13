using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_ThrowableCore : KB_SplashDamageCore
    {

        protected Rigidbody2D rb;

        private void Awake() { rb = GetComponent<Rigidbody2D>(); }

        /// <summary>
        /// Method to throw the projectile with direction and velocity
        /// </summary>
        /// <param name="direction">Direction of the projectile</param>
        /// <param name="velocity">Velocity of the projectile</param>
        protected void Throw(Vector3 direction, float velocity, float timeUntil = 0, bool shouldUseTimer = false)
        {
            rb.velocity = direction * velocity;
            if (shouldUseTimer)
                StartCoroutine(StartTimer(timeUntil));
            OnThrow();
        }

        /// <summary>
        /// Override this method to implement logic upon throwing
        /// </summary>
        protected virtual void OnThrow() { return; }

        /// <summary>
        /// Override this function to implement logic when the timer ends
        /// </summary>
        protected virtual void OnTimerEnd() { return; }

        /// <summary>
        /// Override this function to implement logic when the object makes collision
        /// </summary>
        protected virtual void OnHit(Collision2D collider) { return; }


        private IEnumerator StartTimer(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            OnTimerEnd();
        }

        private void OnCollisionEnter2D(Collision2D collider) { OnHit(collider); }
    }
}
