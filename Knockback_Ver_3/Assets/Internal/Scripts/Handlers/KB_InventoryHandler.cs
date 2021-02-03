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
        //** --CONSTRUCTORS--

        public KB_InventoryHandler() { }
        public KB_InventoryHandler(KB_PlayerController controlledActor) => this.m_controlledActor = controlledActor;

        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES--

        private const int _INVENTORY_SIZE = 4;

        private List<KB_ItemSlot> m_inventorySlots = new List<KB_ItemSlot>();
        private KB_ItemSlot m_pickupSlot = null;
        private KB_PlayerController m_controlledActor = null;
        private int m_currentIndex;
        private int m_newIndex;
        private bool m_canUse = false;
        private bool m_bootstrapped = false;
        private int m_bootstrapLoopCounter = 0;


        //** --METHODS--
        //** --PUBLIC METHODS--

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
                    if (tempSlot.m_itemSlotType == ItemSlotType.Inventory)
                    {
                        m_inventorySlots.Add(tempSlot);
                        tempSlot.SetState(true);
                        tempSlot.SetAction(InventorySlotFunctionCallThrough);
                    }
                    else if (m_pickupSlot == null)
                    {
                        m_pickupSlot = tempSlot;
                        tempSlot.SetState(true);
                        tempSlot.SetAction(PickupSlotFunctionCallThrough);
                    }
                }

                m_inventorySlots = m_inventorySlots.OrderBy(temp => temp.m_slotId).ToList();
                m_canUse = true;
                m_bootstrapped = true;
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
                foreach (var slot in m_inventorySlots)
                {
                    slot.ResetAction();
                    slot.ResetItemSlot();
                    slot.SetState();
                }
                m_pickupSlot.ResetAction();
                m_pickupSlot.ResetItemSlot();
                m_pickupSlot.SetState();

                m_inventorySlots.Clear();
                m_pickupSlot = null;

                m_canUse = false;
                m_bootstrapped = false;
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
            foreach (var slot in m_inventorySlots)
                if (slot.m_isSelected)
                    return slot.m_slotId;
            return -1;
        }

        /// <summary>
        /// This is the method called when the player overlaps with an objects trigger volume
        /// </summary>
        /// <param name="container"></param>
        public void TryPickup(KB_ItemContainer container)
        {
            if (!m_bootstrapped)
            {
                TrySlotLoad();
                if (m_bootstrapLoopCounter == 2)
                {
                    Debug.LogError("BOOTSTRAP LOOP ERROR");
                    return;
                }
                ++m_bootstrapLoopCounter;
                TryPickup(container);
                return;
            }

            if (!m_canUse || m_pickupSlot.m_isFull)
                return;

            m_pickupSlot.SetItemSlot(container);
            SpawnIconAsChild(m_pickupSlot, container.GetIconPrefab());
        }

        /// <summary>
        /// This method is invoked when the player ends the overlap with an objects trigger volume
        /// </summary>
        /// <param name="container"></param>
        public void RemovePickup(KB_ItemContainer container)
        {
            if (!m_canUse || !m_pickupSlot.m_isFull)
                return;

            m_pickupSlot.ResetItemSlot();
            DestroyChildIcon(m_pickupSlot);
        }

        /// <summary>
        /// Call this function to manually add item to the inventory
        /// </summary>
        public void AddItemToInventoryExternally(KB_ItemContainer container, int index)
        {
            if (!m_canUse || index >= m_inventorySlots.Count)
                return;

            m_newIndex = index;
            m_currentIndex = GetCurrentlySelectedSlot();

            if (m_currentIndex == m_newIndex)
            {
                if (m_inventorySlots[m_currentIndex].m_isFull)
                    ItemRemover(m_currentIndex);
                ItemAdder(container, m_newIndex, true);
                EnableItemRoutine(m_inventorySlots[m_newIndex]);
            }
            else
            {
                ItemAdder(container, m_newIndex, false);
            }
        }

        /// <summary>
        /// Method to remove an item from the inventory
        /// </summary>
        public void RemoveItemFromInventory(KB_ItemContainer container, bool canInteract = true)
        {
            if (!m_canUse)
                return;

            KB_ItemSlot targetSlot = m_inventorySlots.Find(x => x.GetContainer() == container);

            if (targetSlot == null)
            {
                Debug.LogError("COULDN'T FIND THE OBJECT");
                return;
            }

            ItemRemover(targetSlot.m_slotId);
        }


        //** --PRIVATE METHODS--

        /// <summary>
        /// This is the function invoked by the inventory slots if the player interacts with it
        /// </summary>
        /// <param name="index"></param>
        private void InventorySlotFunctionCallThrough(int index)
        {
            if (m_newIndex >= m_inventorySlots.Count)
                return;

            m_newIndex = index;
            m_currentIndex = GetCurrentlySelectedSlot();

            if (m_currentIndex == -1)
            {
                m_inventorySlots[m_newIndex].SelectSlot();
                if (m_inventorySlots[m_newIndex].m_isFull)
                    EnableItemRoutine(m_inventorySlots[m_newIndex]);
            }
            else if (m_currentIndex == m_newIndex)
            {
                m_inventorySlots[m_newIndex].DeselectSlot();
                if (m_inventorySlots[m_newIndex].m_isFull)
                    ItemRemover(m_newIndex);
            }
            else
            {
                m_inventorySlots[m_currentIndex].DeselectSlot();
                if (m_inventorySlots[m_currentIndex].m_isFull)
                    DisableItemRoutine(m_inventorySlots[m_currentIndex]);

                m_inventorySlots[m_newIndex].SelectSlot();
                if (m_inventorySlots[m_newIndex].m_isFull)
                    EnableItemRoutine(m_inventorySlots[m_newIndex]);
            }
        }

        /// <summary>
        /// Method invoked by the pickupSlot whenever the player interacts with the pickupSlot
        /// </summary>
        /// <param name="_invalidParameter_"></param>
        private void PickupSlotFunctionCallThrough(int _invalidParameter_)
        {
            if (m_pickupSlot.GetContainer() == null)
                return;

            m_pickupSlot.GetContainer().SetItemUser(m_controlledActor.gameObject);

            int emptyIndex = GetEmptySlot();
            m_currentIndex = GetCurrentlySelectedSlot();

            Debug.Log($"{emptyIndex} : {m_currentIndex}");

            if (emptyIndex == -1)
            {
                if (m_currentIndex != -1)
                {
                    ItemRemover(m_currentIndex);
                    ItemAdder(m_pickupSlot.GetContainer(), m_currentIndex, true);
                    m_pickupSlot.ResetItemSlot();
                    DestroyChildIcon(m_pickupSlot);
                }
                else
                {
                    ItemRemover(0);
                    ItemAdder(m_pickupSlot.GetContainer(), 0, false);
                    m_pickupSlot.ResetItemSlot();
                    DestroyChildIcon(m_pickupSlot);
                }
            }
            else
            {
                if (m_currentIndex == emptyIndex)
                {
                    ItemAdder(m_pickupSlot.GetContainer(), emptyIndex, true);
                    m_pickupSlot.ResetItemSlot();
                    DestroyChildIcon(m_pickupSlot);
                }
                else
                {
                    ItemAdder(m_pickupSlot.GetContainer(), emptyIndex, false);
                    m_pickupSlot.ResetItemSlot();
                    DestroyChildIcon(m_pickupSlot);
                }
            }
        }

        /// <summary>
        /// Removes item from inventory
        /// </summary>
        /// <param name="targetIndex"></param>
        private void ItemRemover(int targetIndex)
        {
            if (targetIndex >= m_inventorySlots.Count)
            {
                Debug.LogError("Out of bounds");
                return;
            }
            DetachItem(m_inventorySlots[targetIndex].GetContainer());
            DestroyChildIcon(m_inventorySlots[targetIndex]);
            RemoveItemRoutine(m_inventorySlots[targetIndex]);
            m_inventorySlots[targetIndex].GetContainer().SetItemUser(null);
            m_inventorySlots[targetIndex].ResetItemSlot();
        }

        /// <summary>
        /// Add an item to the inventory slot
        /// </summary>
        /// <param name="container"></param>
        /// <param name="targetIndex"></param>
        /// <param name="shouldEnable"></param>
        private void ItemAdder(KB_ItemContainer container, int targetIndex, bool shouldEnable)
        {
            if(targetIndex >= m_inventorySlots.Count)
            {
                Debug.LogError("Out of bounds");
                return;
            }
            m_inventorySlots[targetIndex].SetItemSlot(container);
            SpawnIconAsChild(m_inventorySlots[targetIndex], m_inventorySlots[targetIndex].GetContainer().GetIconPrefab());
            AttachItem(m_inventorySlots[targetIndex].GetContainer());
            if (shouldEnable)
                EnableItemRoutine(m_inventorySlots[targetIndex]);
            else
                DisableItemRoutine(m_inventorySlots[targetIndex]);
        }

        /// <summary>
        /// Call this method to attach a weapon to the players weaponSlot
        /// </summary>
        /// <param name="container"></param>
        private void AttachItem(KB_ItemContainer container)
        {
            container.GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            container.GetItem().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            container.GetItem().transform.parent = m_controlledActor.m_cachedWeaponSlot;

            container.GetItem().transform.position = m_controlledActor.m_cachedWeaponSlot.position;
            container.GetItem().transform.rotation = m_controlledActor.m_cachedWeaponSlot.rotation;
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
            foreach (var slot in m_inventorySlots)
            {
                if (slot.m_isFull)
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
            foreach (var slot in m_inventorySlots)
                if (!slot.m_isFull)
                    return slot.m_slotId;
            return -1;
        }

        /// <summary>
        /// Method to spawn icon as child of specific slot
        /// </summary>
        /// <param name="slot">Target slot</param>
        /// <param name="iconPrefab">Icon prefab</param>
        private void SpawnIconAsChild(KB_ItemSlot slot, GameObject iconPrefab)
        {
            if (!slot.m_isFull)
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