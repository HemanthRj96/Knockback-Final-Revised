using UnityEngine;

namespace Knockback.Helpers
{
    /// <summary>
    /// This is container class for items that can be picked up, this class used by ItemInteractor
    /// </summary>
    public class KB_ItemContainer
    {
        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES--

        private GameObject m_item = null;
        private GameObject m_iconPrefab = null;
        private GameObject m_itemUser = null;
        private KB_ItemInteractor m_itemInteractor = null;

        //** --METHODS--
        //** --PUBLIC METHODS

        /// <summary>
        /// Method to setup the item container
        /// </summary>
        /// <param name="pickupManager">Target pickup manager for this item</param>
        /// <param name="item">The item itself</param>
        /// <param name="iconPrefab">Icon prefab for this item</param>
        public void SetupContainer(KB_ItemInteractor pickupManager, GameObject item, GameObject iconPrefab)
        {
            m_itemInteractor = pickupManager;
            m_item = item;
            m_iconPrefab = iconPrefab;
        }

        /// <summary>
        /// Returns the item inside the container
        /// </summary>
        public GameObject GetItem() => m_item;

        /// <summary>
        /// Returns the item icon prefab
        /// </summary>
        /// <returns></returns>
        public GameObject GetIconPrefab() => m_iconPrefab;

        /// <summary>
        /// Returns the pickup manager for this item
        /// </summary>
        /// <returns></returns>
        public KB_ItemInteractor GetPickupManager() => m_itemInteractor;

        /// <summary>
        /// Returns the user of this item
        /// </summary>
        /// <returns></returns>
        public GameObject GetItemUser() => m_itemUser;

        /// <summary>
        /// Method to set the item user
        /// </summary>
        /// <param name="itemUser">The user gameObject</param>
        public void SetItemUser(GameObject itemUser) => this.m_itemUser = itemUser;
    }
}