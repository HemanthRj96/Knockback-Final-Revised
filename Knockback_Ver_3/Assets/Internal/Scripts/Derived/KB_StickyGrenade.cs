using UnityEngine;
using Knockback.Core;

namespace Knockback.Derived
{
    public class KB_StickyGrenade : KB_ThrowableCore
    {
        //todo: Commenting :: StickyGrenade
        //todo: Network implementation
        [Header("Sticky grenade backend settings")]
        [Space]

        [SerializeField] private float throwVelocity = 5;
        [SerializeField] private float maxDamageRadius = 5;
        [SerializeField] private float maxDamageAmount = 100;
        [SerializeField] private float timeUntilExplosion = 4;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask layerMask;

        public bool canUse { get; set; } = false;

        public void UseItem(GameObject source)
        {
            if (!canUse)
                return;
            RemoveFromInventory(source);
            Throw(transform.rotation * Vector2.right, throwVelocity, timeUntilExplosion, true);
        }

        protected override void OnHit(Collision2D collider)
        {
            // Do some checking if necessary
            transform.position = collider.transform.position;
            transform.parent = collider.transform;
        }

        protected override void OnFinishSplashDamage()
        {
            //todo: Change the sprite inside the sprite renderer
            Destroy(gameObject, 0.5f);
        }

        protected override void OnTimerEnd()
        {
            //todo: Add the explosion effect here
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
            KB_PlayerInventoryHandler inventory;
            if (source.TryGetComponent(out inventory))
                inventory.RemoveItemFromInventory(gameObject);
        }
    }
}