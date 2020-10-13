﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_SplashDamageCore : MonoBehaviour
    {
        /// <summary>
        /// Contains all the overlapped objects
        /// </summary>
        protected Dictionary<GameObject, float> overlappedObjects = new Dictionary<GameObject, float>();

        /// <summary>
        /// Invoke this method to apply splash damage at any point
        /// </summary>
        /// <param name="origin">Origin of the splash damage</param>
        /// <param name="maxDamageRadius">Maximum damage radius</param>
        /// <param name="maxDamage">Maximum damage</param>
        /// <param name="layerMask">Layer mask</param>
        protected void ApplySplashDamage(Vector3 origin, float maxDamageRadius, float maxDamage, LayerMask layerMask)
        {
            Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(origin, maxDamageRadius, layerMask);
            IDamage damageHandle = null;

            foreach (Collider2D collider in overlappingColliders)
            {

                if (collider.gameObject == null)
                    continue;
                if (!collider.gameObject.TryGetComponent(out damageHandle))
                    continue;

                Debug.Log($"Collided gameobject : {collider.gameObject}");

                GameObject overlappedGameObject = collider.gameObject;

                Vector3 closestPoint = new Vector3(collider.ClosestPoint(origin).x, collider.ClosestPoint(origin).y, origin.z);
                float distanceFromCollider = Vector3.Distance(closestPoint, origin);
                float damagePercent = Mathf.InverseLerp(0, maxDamageRadius, distanceFromCollider);
                float finalDamage = maxDamage * damagePercent;

                Debug.LogError($"Collided object={overlappedGameObject}|Distance from collider={distanceFromCollider}|Closest point={closestPoint}|Damage percent={damagePercent}|Final damage={finalDamage}");

                damageHandle.AddDamage(finalDamage);
                overlappedObjects.Add(overlappedGameObject, finalDamage);
            }

            OnFinishSplashDamage();
        }

        /// <summary>
        /// Call this function to apply basic lingering damage to all overlapped actors
        /// </summary>
        /// <param name="damageReducePercent">Percentage of reduction in the received damage (0-1)</param>
        /// <param name="decayRate">Rate of decay of reduced damage (0-1)</param>
        /// <param name="iteration">Number of times the damage should be applied</param>
        /// <param name="interval">Interval between damage</param>
        protected IEnumerator ApplyBasicLingeringDamage(float damageReducePercent, float decayRate, int iteration = 4, float interval = 0.8f)
        {
            while (iteration > 0)
            {
                foreach (KeyValuePair<GameObject, float> overlappedObject in overlappedObjects)
                {
                    try
                    {
                        overlappedObject.Key.GetComponent<IDamage>().AddDamage(overlappedObject.Value * damageReducePercent);
                    }
                    catch (System.Exception exc)
                    {
                        Debug.Log($"Exception occured : {exc}");
                        continue;
                    }
                    damageReducePercent *= decayRate;
                }
                --iteration;
                yield return new WaitForSecondsRealtime(interval);
                yield return null;
            }
            OnFinishLingeringDamage();
        }

        /// <summary>
        /// Override this function if you have to do anything after the explosion
        /// </summary>
        protected virtual void OnFinishSplashDamage() { return; }

        /// <summary>
        /// Override this function if you have to do anything after the lingering damage
        /// </summary>
        protected virtual void OnFinishLingeringDamage() { return; }

    }
}