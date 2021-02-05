using UnityEngine;
using Knockback.Core;
using System.Collections.Generic;
using Knockback.Controllers;
using Knockback.Utility;
using Knockback.Helpers;

namespace Knockback.Derived
{
    public class KB_Molotov : KB_ThrowableCore, IUsableEntity
    {
        //todo: Apply flame animation
        //todo: Network syncing
        [Header("Molotov backend settings")]
        [Space]

        [SerializeField] private float throwVelocity = 12;
        [SerializeField] private float maxDamageRadius = 4;
        [SerializeField] private float maxDamage = 45;
        [SerializeField] private float damageReductionPercentage = 0.85f;
        [SerializeField] private float decayRate = 0.85f;
        [SerializeField] private int iteration = 5;
        [SerializeField] private float timeDelay = 0.7f;
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask layerMask;

        public bool i_canUse { get; set; } = false;

        public void UseItem(GameObject source)
        {
            if (!i_canUse)
                return;
            base.m_source = source;
            RemoveFromInventory(source);
            Throw(transform.rotation * Vector2.right, throwVelocity);
        }

        protected override void OnHit(Collision2D collider)
        {
            //todo: Play molotov exploding animation here
            //todo: Add flames to players if the molotov hits a player or add the flames to the object it hits
            if (collider.gameObject.GetComponent<KB_PlayerController>() == null)
            {
                //todo: Spawn static flame on that spot which it hits
            }
            ApplySplashDamage(transform.position, maxDamageRadius, maxDamage, layerMask);
        }

        protected override void OnFinishLingeringDamage()
        {
            //todo: Update sprite
            Destroy(gameObject, 0.5f);
        }

        protected override void OnFinishSplashDamage()
        {
            foreach (KeyValuePair<GameObject, float> overlappedObject in m_overlappedObjects)
            {
                // Implement flame attach
                AddFlameEffect();
            }
            StartCoroutine(ApplyBasicLingeringDamage(damageReductionPercentage, decayRate, iteration, timeDelay));
        }

        private void AddFlameEffect()
        {
            //todo: Call this function to attach flame objects onto affected items
        }

        private void RemoveFromInventory(GameObject source)
        {
            if (source == null)
                return;
            KB_InventoryHandler inventoryHandler = null;
            KB_ItemInteractor pickupManager = GetComponent<KB_ItemInteractor>();
            if (source.GetComponent<KB_PlayerController>())
            {
                inventoryHandler = source.GetComponent<KB_PlayerController>().inventoryHandler;
                inventoryHandler.RemoveItemFromInventory(pickupManager.GetItemContainer(), false);
            }
        }
    }
}