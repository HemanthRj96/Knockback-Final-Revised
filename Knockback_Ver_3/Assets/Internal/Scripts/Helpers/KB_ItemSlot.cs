using Knockback.Handlers;
using Knockback.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Knockback.Helpers
{
    public class KB_ItemSlot : MonoBehaviour
    {
        #region --Attributes--


        [Header("Item slot deafult settings")]
        [Space]

        public ItemSlotType itemSlotType = 0;
        public int slotId = 0;

        [SerializeField] private Image cachedImage = null;
        [SerializeField] private Button cachedButton = null;
        [SerializeField] private Sprite onSelectSprite = null;
        [SerializeField] private Sprite onDeselectSprite = null;

        private KB_ItemContainer itemContainer = null;
        private Action<int> buttonClickAction = delegate { };

        public bool isFull => itemContainer != null;
        public bool isSelected { get; private set; } = false;


        #endregion --Attributes--

        #region --Private functions--


        private void Awake() => BootStrap();

        private void BootStrap()
        {
            cachedButton.onClick.AddListener(OnButtonClick);
            onDeselectSprite = cachedImage.sprite;
            KB_ReferenceHandler.Add(this);
            SetState(false);
        }

        private void SwitchSprites(Sprite targetSprite)
        {
            if (cachedImage == null)
                return;
            cachedImage.sprite = targetSprite;
        }


        #endregion --Private functions--

        #region --Public functions--


        public void SelectSlot()
        {
            // Debug.Log("Select slot " + slotId);
            SwitchSprites(onSelectSprite);
            isSelected = true;
        }

        public void DeselectSlot()
        {
            // Debug.Log("Deselect slot " + slotId);
            SwitchSprites(onDeselectSprite);
            isSelected = false;
        }

        public void SetState(bool isActive = false) => gameObject.SetActive(isActive);

        public void SetAction(Action<int> action) => buttonClickAction = action;
        public void ResetAction() => buttonClickAction = delegate { };

        public void SetItemSlot(KB_ItemContainer container) => itemContainer = container;
        public void ResetItemSlot() => itemContainer = null;

        public KB_ItemContainer GetContainer() => itemContainer;
        public Button GetButton() => cachedButton;

        public void OnButtonClick() => buttonClickAction.Invoke(slotId);


        #endregion --Public functions--
    }
}
