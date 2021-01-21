using System;
using System.Collections.Generic;
using System.Linq;
using Knockback.Controllers;
using Knockback.Handlers;
using Knockback.Utility;
using UnityEngine;

namespace Knockback.Helpers
{
    public class KB_InventoryHandler
    {
        public KB_InventoryHandler() { }
        public KB_InventoryHandler(KB_PlayerController controlledActor) => this.controlledActor = controlledActor;

        private const int _INVENTORY_SIZE = 4;

        //** Player controller
        private KB_PlayerController controlledActor = null;

        //** Item Buffer
        private List<KB_ItemContainer> itemBuffer = new List<KB_ItemContainer>();
        private KB_ItemContainer activeItem = null;

        //** Item Slots
        private List<KB_ItemSlot> inventorySlots = new List<KB_ItemSlot>();
        private KB_ItemSlot pickupSlot = null;

        //** Other variables
        private int currentIndex;
        private int newIndex;
        private bool canUse = false;
        private bool bootstrapped = false;
        private int bootstrapLoopCounter = 0;

        /// <summary>
        /// Method to initialize pickup and inventory slots
        /// </summary>
        public void TrySlotLoad()
        {
            List<KB_ItemSlot> tempSlots = new List<KB_ItemSlot>();

            if (KB_ReferenceHandler.GetReferences(out tempSlots))
            {
                foreach (var tempSlot in tempSlots)
                {
                    if (tempSlot.itemSlotType == ItemSlotType.Inventory)
                    {
                        inventorySlots.Add(tempSlot);
                        tempSlot.SetState(true);
                        tempSlot.SetAction(InventorySlotFunctionCallThrough);
                    }
                    else if (pickupSlot == null)
                    {
                        pickupSlot = tempSlot;
                        tempSlot.SetState(true);
                        tempSlot.SetAction(PickupSlotFunctionCallThrough);
                    }
                }

                inventorySlots = inventorySlots.OrderBy(temp => temp.slotId).ToList();
                canUse = true;
                bootstrapped = true;
            }
            else
                Debug.Log("Unable to find slots from reference handler");
        }

        /// <summary>
        /// Method to clear all the slots and references
        /// </summary>
        public void ClearAllSlots()
        {
            try
            {
                foreach (var slot in inventorySlots)
                {
                    slot.ResetAction();
                    slot.ResetItemSlot();
                    slot.SetState();
                }
                pickupSlot.ResetAction();
                pickupSlot.ResetItemSlot();
                pickupSlot.SetState();

                inventorySlots.Clear();
                pickupSlot = null;

                canUse = false;
                bootstrapped = false;
            }
            catch (Exception excc)
            {
                Debug.LogWarning($"Found an excpetion {excc}");
            }
        }

        /// <summary>
        /// Returns the currently selected slot
        /// </summary>
        /// <returns>Returns -1 if there is no slot selected</returns>
        public int GetCurrentlySelectedSlot()
        {
            foreach (var slot in inventorySlots)
                if (slot.isSelected)
                    return slot.slotId;
            return -1;
        }

        /// <summary>
        /// This is the method called when the player overlaps with an objects trigger volume
        /// </summary>
        /// <param name="container"></param>
        public void TryPickup(KB_ItemContainer container)
        {
            if (!bootstrapped)
            {
                TrySlotLoad();
                if (bootstrapLoopCounter == 2)
                {
                    Debug.LogError("BOOTSTRAP LOOP ERROR");
                    return;
                }
                ++bootstrapLoopCounter;
                TryPickup(container);
                return;
            }

            if (!canUse || pickupSlot.isFull)
                return;

            pickupSlot.SetItemSlot(container);
            SpawnIconAsChild(pickupSlot, container.GetIconPrefab());
        }

        /// <summary>
        /// This method is invoked when the player ends the overlap with an objects trigger volume
        /// </summary>
        /// <param name="container"></param>
        public void RemovePickup(KB_ItemContainer container)
        {
            if (!canUse || !pickupSlot.isFull)
                return;

            pickupSlot.ResetItemSlot();
            DestroyChildIcon(pickupSlot);
        }

        /// <summary>
        /// Call this function to manually add item to the inventory
        /// </summary>
        public void AddItemToInventoryExternally(KB_ItemContainer container, int index)
        {
            if (!canUse || index >= inventorySlots.Count)
                return;

            newIndex = index;
            currentIndex = GetCurrentlySelectedSlot();

            if (currentIndex == newIndex)
            {
                if (inventorySlots[currentIndex].isFull)
                    ItemRemover(currentIndex);
                ItemAdder(container, newIndex, true);
                EnableItemRoutine(inventorySlots[newIndex]);
            }
            else
            {
                ItemAdder(container, newIndex, false);
            }
        }

        /// <summary>
        /// Method to remove an item from the inventory
        /// </summary>
        public void RemoveItemFromInventory(KB_ItemContainer container, bool canInteract = true)
        {
            if (!canUse)
                return;

            KB_ItemSlot targetSlot = inventorySlots.Find(x => x.GetContainer() == container);

            if (targetSlot == null)
            {
                Debug.LogError("COULDN'T FIND THE OBJECT");
                return;
            }

            ItemRemover(targetSlot.slotId);
        }

        /// <summary>
        /// This is the function invoked by the inventory slots if the player interacts with it
        /// </summary>
        /// <param name="index"></param>
        private void InventorySlotFunctionCallThrough(int index)
        {
            if (newIndex >= inventorySlots.Count)
                return;

            newIndex = index;
            currentIndex = GetCurrentlySelectedSlot();

            if (currentIndex == -1)
            {
                inventorySlots[newIndex].SelectSlot();
                if (inventorySlots[newIndex].isFull)
                    EnableItemRoutine(inventorySlots[newIndex]);
            }
            else if (currentIndex == newIndex)
            {
                inventorySlots[newIndex].DeselectSlot();
                if (inventorySlots[newIndex].isFull)
                    ItemRemover(newIndex);
            }
            else
            {
                inventorySlots[currentIndex].DeselectSlot();
                if (inventorySlots[currentIndex].isFull)
                    DisableItemRoutine(inventorySlots[currentIndex]);

                inventorySlots[newIndex].SelectSlot();
                if (inventorySlots[newIndex].isFull)
                    EnableItemRoutine(inventorySlots[newIndex]);
            }
        }

        /// <summary>
        /// Method invoked by the pickupSlot whenever the player interacts with the pickupSlot
        /// </summary>
        /// <param name="_invalidParameter_"></param>
        private void PickupSlotFunctionCallThrough(int _invalidParameter_)
        {
            if (pickupSlot.GetContainer() == null)
                return;

            pickupSlot.GetContainer().SetItemUser(controlledActor.gameObject);

            int emptyIndex = GetEmptySlot();
            currentIndex = GetCurrentlySelectedSlot();

            Debug.Log($"{emptyIndex} : {currentIndex}");

            if (emptyIndex == -1)
            {
                if (currentIndex != -1)
                {
                    ItemRemover(currentIndex);
                    ItemAdder(pickupSlot.GetContainer(), currentIndex, true);
                    pickupSlot.ResetItemSlot();
                    DestroyChildIcon(pickupSlot);
                }
                else
                {
                    ItemRemover(0);
                    ItemAdder(pickupSlot.GetContainer(), 0, false);
                    pickupSlot.ResetItemSlot();
                    DestroyChildIcon(pickupSlot);
                }
            }
            else
            {
                if (currentIndex == emptyIndex)
                {
                    ItemAdder(pickupSlot.GetContainer(), emptyIndex, true);
                    pickupSlot.ResetItemSlot();
                    DestroyChildIcon(pickupSlot);
                }
                else
                {
                    ItemAdder(pickupSlot.GetContainer(), emptyIndex, false);
                    pickupSlot.ResetItemSlot();
                    DestroyChildIcon(pickupSlot);
                }
            }
        }

        /// <summary>
        /// Removes item from inventory
        /// </summary>
        /// <param name="targetIndex"></param>
        private void ItemRemover(int targetIndex)
        {
            if (targetIndex >= inventorySlots.Count)
            {
                Debug.LogError("Out of bounds");
                return;
            }
            DetachItem(inventorySlots[targetIndex].GetContainer());
            DestroyChildIcon(inventorySlots[targetIndex]);
            RemoveItemRoutine(inventorySlots[targetIndex]);
            inventorySlots[targetIndex].GetContainer().SetItemUser(null);
            inventorySlots[targetIndex].ResetItemSlot();
        }

        /// <summary>
        /// Add an item to the inventory slot
        /// </summary>
        /// <param name="container"></param>
        /// <param name="targetIndex"></param>
        /// <param name="shouldEnable"></param>
        private void ItemAdder(KB_ItemContainer container, int targetIndex, bool shouldEnable)
        {
            if(targetIndex >= inventorySlots.Count)
            {
                Debug.LogError("Out of bounds");
                return;
            }
            inventorySlots[targetIndex].SetItemSlot(container);
            SpawnIconAsChild(inventorySlots[targetIndex], inventorySlots[targetIndex].GetContainer().GetIconPrefab());
            AttachItem(inventorySlots[targetIndex].GetContainer());
            if (shouldEnable)
                EnableItemRoutine(inventorySlots[targetIndex]);
            else
                DisableItemRoutine(inventorySlots[targetIndex]);
        }

        /// <summary>
        /// Call this method to attach a weapon to the players weaponSlot
        /// </summary>
        /// <param name="container"></param>
        private void AttachItem(KB_ItemContainer container)
        {
            container.GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            container.GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            container.GetItem().transform.parent = controlledActor.cachedWeaponSlot;

            container.GetItem().transform.position = controlledActor.cachedWeaponSlot.position;
            container.GetItem().transform.rotation = controlledActor.cachedWeaponSlot.rotation;
        }

        /// <summary>
        /// Call this method to detach weapon from the players weaponSlot
        /// </summary>
        /// <param name="container"></param>
        private void DetachItem(KB_ItemContainer container)
        {
            container.GetItem().transform.parent = null;
            container.GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }


        /// <summary>
        /// This method can be invoked to manually remove all the items in the player inventory
        /// </summary>
        private void DiscardAllInventoryItems()
        {
            int index = 0;
            foreach (var slot in inventorySlots)
            {
                if (slot.isFull)
                {
                    DestroyChildIcon(slot);
                    DetachItem(slot.GetContainer());
                    slot.GetContainer().GetPickupManager().EnableObject();
                    slot.GetContainer().GetPickupManager().SetUsability(false);
                    slot.GetContainer().GetPickupManager().SetInteractability(true);
                    slot.ResetItemSlot();
                }
                index++;
            }
        }

        /// <summary>
        /// This method runs the remove routine for an item
        /// </summary>
        /// <param name="item"></param>
        private void RemoveItemRoutine(KB_ItemSlot item, bool canInteract = true)
        {
            item.GetContainer().GetPickupManager().EnableObject();
            item.GetContainer().GetPickupManager().SetUsability(false);
            item.GetContainer().GetPickupManager().SetInteractability(canInteract);
        }

        /// <summary>
        /// This method runs the enable routine for an item
        /// </summary>
        /// <param name="item"></param>
        private void EnableItemRoutine(KB_ItemSlot item)
        {
            item.GetContainer().GetPickupManager().EnableObject();
            item.GetContainer().GetPickupManager().SetUsability(true);
            item.GetContainer().GetPickupManager().SetInteractability(false);
        }

        /// <summary>
        /// This method runs the diable routine for an item
        /// </summary>
        /// <param name="item"></param>
        private void DisableItemRoutine(KB_ItemSlot item)
        {
            item.GetContainer().GetPickupManager().SetUsability(false);
            item.GetContainer().GetPickupManager().SetInteractability(false);
            item.GetContainer().GetPickupManager().DisableObject();
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
        /// Method to spawn icon as child of specific slot
        /// </summary>
        /// <param name="slot">Target slot</param>
        /// <param name="iconPrefab">Icon prefab</param>
        private void SpawnIconAsChild(KB_ItemSlot slot, GameObject iconPrefab)
        {
            if (!slot.isFull)
                return;
            GameObject.Instantiate(iconPrefab, slot.transform);
        }

        /// <summary>
        /// Destroys the child gameobject spawned under the inventorySlot
        /// </summary>
        /// <param name="slot"></param>
        private void DestroyChildIcon(KB_ItemSlot slot)
        {
            try { GameObject.Destroy(slot.transform.GetChild(0).gameObject); }
            catch (Exception exc) { Debug.Log(exc); }
        }

    }
}