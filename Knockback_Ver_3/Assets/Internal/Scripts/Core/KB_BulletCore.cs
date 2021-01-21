using Knockback.Helpers;
using Knockback.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KB_BulletCore : MonoBehaviour, IUsableEntity
{
    //** --ATTRIBUTES--    
    //** --PRIVATE ATTRIBUTES--

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 direction;
    private bool shouldIgnoreSelf = true;
    private Action<GameObject> functionCallback = null;
    private List<GameObject> collidedObjects = new List<GameObject>();

    //** --PUBLIC ATTRIBUTES--

    public bool canUse { get; set; } = true;
    public KB_BulletModifier bulletModifier = null;


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
    public void SetBulletParameters(float impactDamage, Vector2 direction, float velocity, bool shouldIgnoreSelf = true, Action<GameObject> functionCallback = null)
    {
        this.direction = direction;
        this.shouldIgnoreSelf = shouldIgnoreSelf;
        this.functionCallback = functionCallback;
        bulletModifier.SetDamageAndSpeed(impactDamage, velocity);
    }

    /// <summary>
    /// Method which returns all the collided objects of this bullet
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetCollidedObjects() => collidedObjects;

    //** --PRIVATE METHODS--

    /// <summary>
    /// Do this when the bullet is destroyed
    /// </summary>
    private void OnDestroy() => StopAllCoroutines();

    /// <summary>
    /// Assign the bullet modifier
    /// </summary>
    private void Awake() => bulletModifier = GetComponent<KB_BulletModifier>();

    /// <summary>
    /// Do this on disable
    /// </summary>
    private void OnDisable()
    {
        functionCallback = null;
        collidedObjects.Clear();
    }

    /// <summary>
    /// Method called when the bullet is "fired"
    /// </summary>
    private IEnumerator Fire(GameObject source)
    {
        // Propel the bullet
        bulletModifier.StartBulletTranslation();

        while (bulletModifier.m_canDetect)
        {
            pointA = transform.position;
            yield return new WaitForSecondsRealtime(0.001f);
            pointB = transform.position;

            RaycastHit2D hit = bulletModifier.DoRayCast(pointA, transform.rotation * Vector2.right, Vector2.Distance(pointA, pointB));

            if (hit.collider == null)
                continue;
            if (shouldIgnoreSelf && hit.collider.gameObject == source)
            {
                if (bulletModifier.ShouldRicochet())
                    DisableSelfIgnore();
                continue;
            }

            collidedObjects.Add(hit.collider.gameObject);
            functionCallback?.Invoke(hit.collider.gameObject);
            yield return new WaitUntil(() => OnHit(hit));
        }
    }

    /// <summary>
    /// This is the method invoked when the bullet hits an obstacle
    /// </summary>
    /// <param name="hit"></param>
    private bool OnHit(RaycastHit2D hit) => bulletModifier.OnHit(hit);

    /// <summary>
    /// Method changes the shouldIngoreSelf to false
    /// </summary>
    private void DisableSelfIgnore() => shouldIgnoreSelf = false;
}
