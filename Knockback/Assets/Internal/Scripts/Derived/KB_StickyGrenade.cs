using UnityEngine;
using Knockback.Core;
using Knockback.Helpers;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Derived
{
    //todo: Should implement animations and FX - KB_StickyGrenade
    //todo: Network implementation - KB_StickyGrenade
    public class KB_StickyGrenade : KB_ThrowableCore, IUsableEntity
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Sticky grenade backend settings")]
        [Space]

        [SerializeField] private float throwVelocity = 5;
        [SerializeField] private float maxDamageRadius = 5;
        [SerializeField] private float maxDamageAmount = 100;
        [SerializeField] private float timeUntilExplosion = 4;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
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
            base.m_source = source;
            RemoveFromInventory(source);
            Throw(transform.rotation * Vector2.right, throwVelocity, timeUntilExplosion, true);
        }

        //** --PROTECTED METHODS--
        
        /// <summary>
        /// Attach the sticky grenade to the point of collision
        /// </summary>
        /// <param name="collider"></param>
        protected override void OnHit(Collision2D collider)
        {
            // Do some checking if necessary
            transform.position = collider.transform.position;
            transform.parent = collider.transform;
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
        /// Explode and add explosion effects here
        /// </summary>
        protected override void OnTimerEnd()
        {
            //Add the explosion effect here
            ApplySplashDamage(transform.position, maxDamageRadius, maxDamageAmount, layerMask);
        }

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