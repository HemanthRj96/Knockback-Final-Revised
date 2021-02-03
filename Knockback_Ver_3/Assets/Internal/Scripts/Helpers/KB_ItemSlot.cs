using Knockback.Handlers;
using Knockback.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Knockback.Helpers
{
    /// <summary>
    /// Class holds the resposibilty for UI handling for inventory and pickup items
    /// </summary>
    public class KB_ItemSlot : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

        [Header("Item slot deafult settings")]
        [Space]
        public ItemSlotType m_itemSlotType = 0;
        public int m_slotId = 0;

        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private Image m_cachedImage = null;
        [SerializeField] private Button m_cachedButton = null;
        [SerializeField] private Sprite m_onSelectSprite = null;
        [SerializeField] private Sprite m_onDeselectSprite = null;

        //** --PRIVATE ATTRIBUTES--

        private KB_ItemContainer m_itemContainer = null;
        private Action<int> m_buttonClickAction = delegate { };

        //** --PUBLIC REFERENCES--

        public bool m_isFull => m_itemContainer != null;
        public bool m_isSelected { get; private set; } = false;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to select this slot
        /// </summary>
        public void SelectSlot()
        {
            // Debug.Log("Select slot " + slotId);
            SwitchSprites(m_onSelectSprite);
            m_isSelected = true;
        }

        /// <summary>
        /// Method to deselect this slot
        /// </summary>
        public void DeselectSlot()
        {
            // Debug.Log("Deselect slot " + slotId);
            SwitchSprites(m_onDeselectSprite);
            m_isSelected = false;
        }

        /// <summary>
        /// Method to activate or deactivate this gameObject
        /// </summary>
        /// <param name="isActive"></param>
        public void SetState(bool isActive = false) => gameObject.SetActive(isActive);

        /// <summary>
        /// Method to set the action
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(Action<int> action) => m_buttonClickAction = action;

        /// <summary>
        /// Method to reset the action
        /// </summary>
        public void ResetAction() => m_buttonClickAction = delegate { };

        /// <summary>
        /// Method to set the item slot
        /// </summary>
        /// <param name="container"></param>
        public void SetItemSlot(KB_ItemContainer container) => m_itemContainer = container;

        /// <summary>
        /// Method to reset the item slot
        /// </summary>
        public void ResetItemSlot() => m_itemContainer = null;

        /// <summary>
        /// Returns the item container
        /// </summary>
        public KB_ItemContainer GetContainer() => m_itemContainer;

        /// <summary>
        /// Returns the button component of this ItemSlot
        /// </summary>
        /// <returns></returns>
        public Button GetButton() => m_cachedButton;

        //** --PRIVATE METHODS--

        /// <summary>
        /// Call bootstrap on Awake
        /// </summary>
        private void Awake() => BootStrap();

        /// <summary>
        /// Method implements all the bootstrapping routines
        /// </summary>
        private void BootStrap()
        {
            m_cachedButton.onClick.AddListener(OnButtonClick);
            m_onDeselectSprite = m_cachedImage.sprite;
            KB_ReferenceHandler.Add(this);
            SetState(false);
        }

        /// <summary>
        /// This method is used to switch sprites of the itemSlot
        /// </summary>
        /// <param name="targetSprite">Target sprite to use</param>
        private void SwitchSprites(Sprite targetSprite)
        {
            if (m_cachedImage == null)
                return;
            m_cachedImage.sprite = targetSprite;
        }

        /// <summary>
        /// This method is automatically invoked upon buttonClick
        /// </summary>
        private void OnButtonClick() => m_buttonClickAction.Invoke(m_slotId);
    }
}
