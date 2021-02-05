using Knockback.Controllers;
using Knockback.Core;
using Knockback.Helpers;
using Knockback.Utility;
using UnityEngine;

namespace Knockback.Derived
{
    //todo: Should implement animations and FX - KB_HandGrenade
    //todo: Network implementation - KB_HandGrenade
    public class KB_HandGrenade : KB_ThrowableCore, IUsableEntity
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Hand grenade backend settings")]
        [Space]

        [SerializeField] private float m_throwVelocity = 5;
        [SerializeField] private float m_maxDamageRadius = 5;
        [SerializeField] private float m_maxDamageAmount = 110;
        [SerializeField] private Animator m_animator;
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private LayerMask m_layerMask;

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
            Throw(transform.rotation * Vector2.right, m_throwVelocity, 3, true);
        }

        //** --OVERRIDED METHODS--

        /// <summary>
        /// Change sprite and destroy the object from the master pooler object
        /// </summary>
        protected override void OnFinishSplashDamage()
        {
            //Change the sprite inside the sprite renderer

            Destroy(gameObject, 0.5f);
        }

        /// <summary>
        /// Add explosion FX and call the ApplySplashDamage method
        /// </summary>
        protected override void OnTimerEnd()
        {
            //Add the explosion effect here

            ApplySplashDamage(transform.position, m_maxDamageRadius, m_maxDamageAmount, m_layerMask);
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
