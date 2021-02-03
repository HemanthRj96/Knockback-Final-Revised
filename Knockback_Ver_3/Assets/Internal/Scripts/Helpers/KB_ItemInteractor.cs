using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class implements logic for objects that can be pickkd up and dropped therefore 
    /// attach this class as a component to objects that is meant to be picked up
    /// </summary>
    public class KB_ItemInteractor : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Default backend settings")]
        [Space]
        [SerializeField] private GameObject m_iconPrefab;

        //** --PRIVATE ATTRIBUTES--

        private bool m_isPicked => transform.parent != null;
        private bool m_canInteract = true;
        private KB_InventoryHandler m_bufferedInventoryHandler = null;
        private KB_ItemContainer m_itemContainer = new KB_ItemContainer();


        //** --METHODS-- 
        //** --PUBLIC METHODS--

        /// <summary>
        /// Enables this gameObject
        /// </summary>
        public void EnableObject() => gameObject.SetActive(true);

        /// <summary>
        /// Disables this gameObject
        /// </summary>
        public void DisableObject() => gameObject.SetActive(false);

        /// <summary>
        /// Method to set the usability of this gameObject
        /// </summary>
        /// <param name="canUse">Usability state value</param>
        public void SetUsability(bool canUse) => gameObject.GetComponent<IUsableEntity>().i_canUse = canUse;

        /// <summary>
        /// Method to set the interactability of this gameObject
        /// </summary>
        /// <param name="canInteract">Interactabilty state value</param>
        public void SetInteractability(bool canInteract) => this.m_canInteract = canInteract;

        /// <summary>
        /// Returns the itemContainer
        /// </summary>
        public KB_ItemContainer GetItemContainer() => m_itemContainer;

        //** --PRIVATE METHODS

        /// <summary>
        /// Bootstrap container on Awake
        /// </summary>
        private void Awake() => BootstrapContainer();

        /// <summary>
        /// Sets up the item container
        /// </summary>
        private void BootstrapContainer()
        {
            if (m_iconPrefab == null)
            {
                Debug.LogError("UI HANDLER NOT SETUP");
                m_canInteract = false;
                return;
            }
            m_itemContainer.SetupContainer(this, gameObject, m_iconPrefab);
        }

        /// <summary>
        /// Add to pickupSlot if available
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!m_canInteract && m_isPicked)
                return;
            m_bufferedInventoryHandler = null;
            m_bufferedInventoryHandler = collider.GetComponent<KB_PlayerController>()?.inventoryHandler;
            if (m_bufferedInventoryHandler == null)
                return;

            m_bufferedInventoryHandler.TryPickup(m_itemContainer);
        }

        /// <summary>
        /// Remove this item from pickupSlot if its there
        /// </summary>
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (!m_canInteract && m_isPicked)
                return;
            m_bufferedInventoryHandler = null;
            m_bufferedInventoryHandler = collider.GetComponent<KB_PlayerController>()?.inventoryHandler;
            if (m_bufferedInventoryHandler == null)
                return;

            m_bufferedInventoryHandler.RemovePickup(m_itemContainer);
        }
    }
}