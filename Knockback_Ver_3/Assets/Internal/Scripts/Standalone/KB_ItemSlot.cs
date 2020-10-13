using Knockback.Handlers;
using System;
using UnityEngine;
using UnityEngine.UI;

// todo: Commenting :: ItemSlot
// todo: Implement surface implementation of the hud text like :- setting text, resetting text and enabling/disable text

public class KB_ItemSlot : MonoBehaviour
{
    public enum ItemSlotType { Pickup, Inventory }

    [Header("Item slot deafult settings")]
    [Space]

    public ItemSlotType itemSlotType = 0;
    public int slotId = 0;
    [SerializeField]
    private Image cachedImage = null;
    [SerializeField]
    private Button cachedButton = null;
    [SerializeField]
    private Sprite onSelectSprite = null;
    private Sprite onDeselectSprite = null;

    private GameObject targetItem = null;
    private Action<int> buttonClickAction = delegate { };

    public bool isFull => targetItem != null;
    [HideInInspector]
    public bool isSelected = false;

    private bool buttonClick = false;

    private void Awake()
    {
        cachedButton.onClick.AddListener(OnButtonClick);
        onDeselectSprite = cachedImage.sprite;
        KB_ReferenceHandler.Add(this);
        SetState(false);
    }

    public void SelectSlot()
    {
        cachedImage.sprite = onSelectSprite;
        isSelected = true;
    }

    public void DeselectSlot()
    {
        cachedImage.sprite = onDeselectSprite;
        isSelected = false;
    }

    public void SetState(bool isActive = false) { gameObject.SetActive(isActive); }
    public void SetAction(Action<int> action) { buttonClickAction = action; }
    public void ResetAction() { buttonClickAction = delegate { }; }
    public void SetItemSlot(GameObject item) { targetItem = item; }
    public void ResetItemSlot() { targetItem = null; }
    public GameObject GetItem() { return targetItem; }
    public Button GetButton() { return cachedButton; }

    // todo: Remove the flag check if whenreleasing into mobile edition
    public void OnButtonClick() 
    { 
        if(!buttonClick)
            buttonClickAction.Invoke(slotId);
        buttonClick = !buttonClick;
    }

}
