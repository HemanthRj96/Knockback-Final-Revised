using Knockback.Helpers;
using Knockback.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_BulletCore : MonoBehaviour, IUsableEntity
    {
        //** --ATTRIBUTES--    
        //** --PRIVATE ATTRIBUTES--

        private Vector2 m_pointA;
        private Vector2 m_pointB;
        private Vector2 m_direction;
        private bool m_shouldIgnoreSelf = true;
        private Action<GameObject> m_functionCallback = null;
        private List<GameObject> m_collidedObjects = new List<GameObject>();

        //** --PUBLIC ATTRIBUTES--

        public bool i_canUse { get; set; } = true;
        public KB_BulletModifier m_bulletModifier = null;

        //** --PUBLIC REFERENCES--

        public GameObject m_source { private set; get; }

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method implementation of IUsableEntity interface
        /// </summary>
        /// <param name="source">Object thats using this gameobject</param>
        public void UseItem(GameObject source) => StartCoroutine(Fire(source));

        /// <summary>
        /// Method to set the bullet parameters, call this method before using UseItem method
        /// </summary>
        /// <param name="impactDamage">Impact damage of the bullet</param>
        /// <param name="direction">Direction of the bullet</param>
        /// <param name="velocity">Velocity of the bullet</param>
        public void SetBulletParameters
            (
                float impactDamage,
                Vector2 direction,
                float velocity,
                bool shouldIgnoreSelf = true,
                Action<GameObject> functionCallback = null,
                bool shouldPassthrough = false,
                bool shouldRicochet = false
            )
        {
            this.m_direction = direction;
            this.m_shouldIgnoreSelf = shouldIgnoreSelf;
            this.m_functionCallback = functionCallback;
            m_bulletModifier.SetDamageAndSpeed(impactDamage, velocity);
            if (shouldPassthrough)
                m_bulletModifier.EnablePassthrough();
            else
                m_bulletModifier.DisablePassthrough();
            if (shouldRicochet)
                m_bulletModifier.EnableRicochet();
            else
                m_bulletModifier.DisableRicochet();
        }

        /// <summary>
        /// Method which returns all the collided objects of this bullet
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetCollidedObjects() => m_collidedObjects;

        //** --PRIVATE METHODS--

        /// <summary>
        /// Do this when the bullet is destroyed
        /// </summary>
        private void OnDestroy() => StopAllCoroutines();

        /// <summary>
        /// Assign the bullet modifier
        /// </summary>
        private void Awake() => m_bulletModifier = GetComponent<KB_BulletModifier>();

        /// <summary>
        /// Do this on disable
        /// </summary>
        private void OnDisable()
        {
            m_functionCallback = null;
            m_collidedObjects.Clear();
        }

        /// <summary>
        /// Method called when the bullet is "fired"
        /// </summary>
        private IEnumerator Fire(GameObject source)
        {
            this.m_source = source;
            // Propel the bullet
            m_bulletModifier.StartBulletTranslation();

            while (m_bulletModifier.m_canDetect)
            {
                m_pointA = transform.position;
                yield return null;
                m_pointB = transform.position;

                RaycastHit2D hit = m_bulletModifier.DoRayCast(m_pointA, transform.rotation * Vector2.right, Vector2.Distance(m_pointA, m_pointB));

                if (hit.collider == null)
                    continue;
                if (m_shouldIgnoreSelf && hit.collider.gameObject == source)
                {
                    if (m_bulletModifier.ShouldRicochet())
                        DisableSelfIgnore();
                    continue;
                }

                m_collidedObjects.Add(hit.collider.gameObject);
                m_functionCallback?.Invoke(hit.collider.gameObject);
                yield return new WaitUntil(() => OnHit(hit));
            }
        }

        /// <summary>
        /// This is the method invoked when the bullet hits an obstacle
        /// </summary>
        /// <param name="hit"></param>
        private bool OnHit(RaycastHit2D hit) => m_bulletModifier.OnHit(hit);

        /// <summary>
        /// Method changes the shouldIngoreSelf to false
        /// </summary>
        private void DisableSelfIgnore() => m_shouldIgnoreSelf = false;
    }

}
