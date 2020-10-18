using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Knockback.Handlers;
using Knockback.Controllers;
using Knockback.Utility;

public class KB_PlayerInventoryHandler : MonoBehaviour
{
    //todo: Implement the HUD text handling for bullet count like :- text updating, text hiding and so on
    //todo: Network implementation

    private List<KB_ItemSlot> inventorySlots = new List<KB_ItemSlot>();
    private KB_ItemSlot pickupSlot = null;
    private KB_PlayerController controller = null;
    private bool canUse = false;

    //*** Default functions ***//

    private void Awake()
    {
        TrySlotLoad();
        controller = GetComponent<KB_PlayerController>();
        if (controller == null)
        {
            canUse = false;
            new KBLog("Missing component : KB_PlayerController");
        }
    }

    private void OnDestroy()
    {
        if (!canUse)
            return;
        ClearReferences();
        DiscardAllItems();
    }

    /// <summary>
    /// Returns the index of the current active inventory slot
    /// </summary>
    /// <returns></returns>
    public int GetActiveSlotIndex()
    {
        if (!canUse)
            return -1;
        foreach (var slot in inventorySlots)
            if (slot.isSelected)
                return slot.slotId;
        inventorySlots[0].SelectSlot();
        return 0;
    }

    /// <summary>
    /// Returns ItemSlot object from the list of inventory
    /// </summary>
    /// <param name="index">The corresponding index of item</param>
    /// <returns></returns>
    public KB_ItemSlot GetItemFromInventory(int index)
    {
        return canUse ? inventorySlots[index] : null;
    }

    /// <summary>
    /// Call this function to add item to the pickup slot
    /// </summary>
    /// <param name="item">The target item gameObject</param>
    public void TryPickup(GameObject item)
    {
        if (!canUse)
            return;
        if (pickupSlot.isFull)
            return;
        pickupSlot.SetItemSlot(item);
        SpawnIconAsChild(pickupSlot);
    }

    /// <summary>
    /// Call this function to remove item from pickup slot if not null
    /// </summary>
    public void TryRemoveFromPickup(GameObject item)
    {
        if (!canUse)
            return;
        if (!pickupSlot.isFull)
            return;
        if (pickupSlot.GetItem() == item)
        {
            pickupSlot.ResetItemSlot();
            DestroyChildIcon(pickupSlot);
        }
    }

    /// <summary>
    /// Call this function to manually add item to the inventory
    /// </summary>
    public void AddItemToInventory(GameObject item, int slotId)
    {
        if (!canUse)
            return;
        int activeIndex = GetActiveSlotIndex();
        if (activeIndex == slotId)
        {
            if (inventorySlots[slotId].isFull)
            {
                DestroyChildIcon(inventorySlots[slotId]);
                DetachItem(slotId);
                inventorySlots[slotId].ResetItemSlot();

                inventorySlots[slotId].SetItemSlot(item);
                AttachItem(slotId);
                SpawnIconAsChild(inventorySlots[slotId]);
                inventorySlots[slotId].GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true);
                inventorySlots[slotId].SelectSlot();
            }
            else
            {
                inventorySlots[slotId].SetItemSlot(item);
                AttachItem(slotId);
                SpawnIconAsChild(inventorySlots[slotId]);
                inventorySlots[activeIndex].GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true);
                inventorySlots[slotId].SelectSlot();
            }
        }
        else
        {
            inventorySlots[slotId].SetItemSlot(item);
            AttachItem(slotId);
            SpawnIconAsChild(inventorySlots[slotId]);
            inventorySlots[slotId].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(false);
        }
    }

    /// <summary>
    /// Method to remove an item from the inventory
    /// </summary>
    public void RemoveItemFromInventory(GameObject item, bool canInteract = true)
    {
        if (!canUse)
            return;
        foreach (var slot in inventorySlots)
        {
            if (slot.GetItem() == item)
            {
                DetachItem(slot.slotId);
                DestroyChildIcon(slot);
                slot.GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true, false, canInteract);
                if (slot.isSelected)
                    slot.DeselectSlot();
                slot.ResetItemSlot();
            }
        }
    }

    //*** Private methods ***//

    /// <summary>
    /// Method to initialize pickup and inventory slots
    /// </summary>
    private void TrySlotLoad()
    {
        List<KB_ItemSlot> tempSlots = new List<KB_ItemSlot>();
        try
        {
            if (KB_ReferenceHandler.GetReferences(out tempSlots))
            {
                foreach (var tempSlot in tempSlots)
                {
                    if (tempSlot.itemSlotType == ItemSlotType.Inventory)
                    {
                        inventorySlots.Add(tempSlot);
                        tempSlot.SetState(true);
                        tempSlot.SetAction(ISFunctionCallThrough);
                    }
                    else if (pickupSlot == null)
                    {
                        pickupSlot = tempSlot;
                        tempSlot.SetState(true);
                        tempSlot.SetAction(PSFunctionCallThrough);
                    }
                }

                inventorySlots = inventorySlots.OrderBy(temp => temp.slotId).ToList();
                canUse = true;
            }
            else
                Debug.Log("Unable to find slots from reference handler");
        }
        catch (Exception) { new KBLog("Add reference handler"); }
    }

    /// <summary>
    /// Returns empty slot
    /// </summary>
    /// <returns></returns>
    private int GetEmptySlot()
    {
        foreach (var slot in inventorySlots)
            if (!slot.isFull)
                return slot.slotId;
        return -1;
    }

    /// <summary>
    /// ItemSlot button OnClick action
    /// </summary>
    /// <param name="slotId"></param>
    private void PSFunctionCallThrough(int slotId)
    {
        //todo: Implement a checking for empty slot function call
        int emptyIndex = GetEmptySlot();
        if (pickupSlot.GetItem() == null)
            return;
        if (emptyIndex != -1)
        {
            inventorySlots[emptyIndex].SetItemSlot(pickupSlot.GetItem());
            SpawnIconAsChild(inventorySlots[emptyIndex]);
            AttachItem(emptyIndex);

            if (emptyIndex == GetActiveSlotIndex())
            {
                inventorySlots[emptyIndex].GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true);
                inventorySlots[emptyIndex].SelectSlot();
            }
            else
            {
                inventorySlots[emptyIndex].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(false);
                inventorySlots[emptyIndex].DeselectSlot();
            }

            pickupSlot.ResetItemSlot();
            DestroyChildIcon(pickupSlot);
        }
        else
        {
            int activeIndex = GetActiveSlotIndex();

            inventorySlots[activeIndex].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(true, false, true);
            DetachItem(activeIndex);
            DestroyChildIcon(inventorySlots[activeIndex]);
            inventorySlots[activeIndex].ResetItemSlot();

            inventorySlots[activeIndex].SetItemSlot(pickupSlot.GetItem());
            SpawnIconAsChild(inventorySlots[activeIndex]);
            AttachItem(activeIndex);
            inventorySlots[activeIndex].GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true);
            inventorySlots[activeIndex].SelectSlot();

            pickupSlot.ResetItemSlot();
            DestroyChildIcon(pickupSlot);
        }
    }

    /// <summary>
    /// ItemSlot button OnClick action
    /// </summary>
    /// <param name="slotId"></param>
    private void ISFunctionCallThrough(int slotId)
    {
        int activeSlot = GetActiveSlotIndex();

        if (inventorySlots[slotId].GetItem() == null)
        {
            if (inventorySlots[activeSlot].GetItem() != null)
                inventorySlots[activeSlot].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(false);
            inventorySlots[activeSlot].DeselectSlot();
            inventorySlots[slotId].SelectSlot();
        }
        else if (slotId == activeSlot)
        {
            inventorySlots[slotId].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(true, false, true);
            DestroyChildIcon(inventorySlots[slotId]);
            DetachItem(slotId);
            inventorySlots[slotId].ResetItemSlot();
        }
        else
        {
            inventorySlots[slotId].GetItem().GetComponent<KB_ItemPickup>().CmdEnableItem(true);
            inventorySlots[slotId].SelectSlot();
            if (inventorySlots[activeSlot].GetItem() != null)
                inventorySlots[activeSlot].GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(false);
            inventorySlots[activeSlot].DeselectSlot();
        }
    }

    /// <summary>
    /// Detaches rigidBody item from the weaponSlot
    /// </summary>
    /// <param name="index"></param>
    private void DetachItem(int index)
    {
        inventorySlots[index].GetItem().transform.parent = null;
        inventorySlots[index].GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    /// Attaches the rigidBody item to the inventorySlot
    /// </summary>
    /// <param name="index"></param>
    private void AttachItem(int index)
    {
        GameObject item = inventorySlots[index].GetItem();
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        //Transform itemTransform = controller.weaponSlot;

        rb.bodyType = RigidbodyType2D.Static;
        rb.bodyType = RigidbodyType2D.Kinematic;

        //item.transform.SetPositionAndRotation(itemTransform.position, itemTransform.rotation);
        //item.transform.SetParent(itemTransform, true);
    }

    /// <summary>
    /// Spawn icon as a child to the item slot
    /// </summary>
    /// <param name="slot"></param>
    private void SpawnIconAsChild(KB_ItemSlot slot)
    {
        if (!slot.isFull)
            return;
        GameObject icon = slot.GetItem().GetComponent<KB_ItemPickup>().iconPrefab;
        Instantiate(icon, slot.transform);
    }

    /// <summary>
    /// Destroy any child icon of the item slot
    /// </summary>
    /// <param name="slot"></param>
    private void DestroyChildIcon(KB_ItemSlot slot)
    {
        try { Destroy(slot.transform.GetChild(0).gameObject); }
        catch (Exception exc) { Debug.Log($"{gameObject} {exc}"); }
    }

    /// <summary>
    /// Method upon invoke every item in the inventory
    /// </summary>
    private void DiscardAllItems()
    {
        int index = 0;
        foreach (var slot in inventorySlots)
        {
            if (slot.isFull)
            {
                DestroyChildIcon(slot);
                DetachItem(index);
                slot.GetItem().GetComponent<KB_ItemPickup>().CmdDisableItem(true, false, true);
                slot.ResetItemSlot();
            }
            index++;
        }
    }

    /// <summary>
    /// Method to nullify references
    /// </summary>
    private void ClearReferences()
    {
        try
        {
            foreach (var slot in inventorySlots)
            {
                slot.ResetAction();
                slot.SetState();
            }
            pickupSlot.ResetAction();
            pickupSlot.SetState();
        }
        catch (Exception excc)
        {
            Debug.LogWarning($"Found an excpetion {excc}");
        }
    }
}