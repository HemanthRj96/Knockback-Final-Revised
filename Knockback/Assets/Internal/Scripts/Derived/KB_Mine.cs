using UnityEngine;
using System.Collections;
using Knockback.Core;
using Knockback.Helpers;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Derived
{
    //todo: Should implement animations and FX - KB_Mine
    //todo: Network implementation - KB_Mine
    public class KB_Mine : KB_ThrowableCore, IUsableEntity
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Hand grenade backend settings")]
        [Space]

        [SerializeField] private float throwVelocity = 5;
        [SerializeField] private float maxDamageRadius = 5;
        [SerializeField] private float maxDamageAmount = 110;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private LayerMask layerMask;

        //** --PUBLIC ATTRIBUTES--

        public bool i_canUse { get; set; } = false;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Interface implementation
        /// </summary>
        /// <param name="source"></param>
        public void UseItem(GameObject source)
        {
            if (!i_canUse)
                return;
            SetSource(source);
            RemoveFromInventory(source);
            Throw(transform.rotation * Vector2.right, throwVelocity);
        }

        /// <summary>
        /// Change sprite and destroy the object from the master pooler object
        /// </summary>
        protected override void OnFinishSplashDamage()
        {
            //Change the sprite inside the sprite renderer
            Destroy(gameObject, 0.5f);
        }

        /// <summary>
        /// Attach the mine to that point
        /// </summary>
        protected override void OnHit(Collision2D collider)
        {
            // Do some checking if necessary
            transform.position = collider.transform.position;
            transform.parent = collider.transform;
        }

        /// <summary>
        /// Explode if any player is closeby and also update animations and FX
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnTriggered(Collider2D collider)
        {
            //Check if theres any player in close vicinity if yes then do the following
            //Play animation and maybe add a 0.5 second delay before explosion
            ApplySplashDamage(transform.position, maxDamageRadius, maxDamageAmount, layerMask);
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Remove this item from the player inventory
        /// </summary>
        /// <param name="source"></param>
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