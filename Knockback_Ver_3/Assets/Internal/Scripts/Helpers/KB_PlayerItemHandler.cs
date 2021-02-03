using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    //todo: Minor implementation - KB_PlayerItemHandler

    /// <summary>
    /// This class implements logic for using any item that is active in the playerInventory
    /// </summary>
    public class KB_PlayerItemHandler
    {
        //** --CONSTRUCTORS--

        public KB_PlayerItemHandler() { }
        public KB_PlayerItemHandler(KB_PlayerController controlledActor) => m_controlledActor = controlledActor;

        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES--

        private KB_PlayerController m_controlledActor;
        private GameObject m_item = null;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to use the active item in the inventory
        /// </summary>
        /// <param name="value"></param>
        public void UseItem(bool value)
        {
            if (!value)
                return;
            if (m_controlledActor.m_cachedWeaponSlot.childCount == 0)
                return;

            // Get the item from the inventory and use it
        }

        /// <summary>
        /// Helper method to use the target item
        /// </summary>
        /// <param name="item">Target item to be used</param>
        private void Use(IUsableEntity item) => item.UseItem(m_controlledActor.gameObject);
    }
}