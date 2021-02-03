using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    /// <summary>
    /// This class implements a basic projectile type splash damage system, therefore this class should be inherited 
    /// for further feature expansion
    /// </summary>
    public class KB_ThrowableCore : KB_SplashDamageCore
    {
        //** --ATTRIBUTES--
        //** --PROTECTED ATTRIBUTES--

        protected Rigidbody2D m_rb;


        //** --METHODS--
        //** --PROTECTED METHODS--

        /// <summary>
        /// Method to throw the projectile with direction and velocity
        /// </summary>
        /// <param name="direction">Direction of the projectile</param>
        /// <param name="velocity">Velocity of the projectile</param>
        protected void Throw(Vector3 direction, float velocity, float timeUntil = 0, bool shouldUseTimer = false)
        {
            m_rb.velocity = direction * velocity;
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
        /// Override this function to implement logic when this object makes a collision with something
        /// </summary>
        protected virtual void OnHit(Collision2D collider) { return; }

        /// <summary>
        /// Override this function to implement logic when an object triggers the volume
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggered(Collider2D collider) { return; }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Initialize rigidBody
        /// </summary>
        private void Awake() => m_rb = GetComponent<Rigidbody2D>();

        /// <summary>
        /// Timer coroutine
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator StartTimer(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            OnTimerEnd();
        }

        /// <summary>
        /// Call OnHit on making collision
        /// </summary>
        /// <param name="collider"></param>
        private void OnCollisionEnter2D(Collision2D collider) => OnHit(collider);

        /// <summary>
        /// Call OnTriggered when triggered by the volume
        /// </summary>
        /// <param name="collider"></param>
        private void OnTriggerEnter2D(Collider2D collider) => OnTriggered(collider);
    }
}
