using Knockback.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knockback.Handlers;
using Mirror;
using Knockback.Utility;


namespace Knockback.Helpers
{
    public class KB_ItemPickup : MonoBehaviour
    {
        //todo: Commenting :: ItemPickup
        //todo: Dynamic hud text implementation for inventory
        //todo: Network implementation

        //*** Backend variables ***//

        [Header("Item pickup backend settings")]
        [Space]

        public GameObject iconPrefab = null;

        //** Variables synchronized over the network
        //[SyncVar]
        private bool canInteract = true;
        //[SyncVar]
        private bool isPicked = false;

        private KB_InventoryHandler inventoryHandler = null;
        private KB_PickupManager pickupManager = null;


        private void Awake() => pickupManager = gameObject.GetComponent<KB_PickupManager>();

        public void EnableItem(bool isActive, bool isPicked = true, bool canInteract = false)
        {
            this.canInteract = canInteract;
            this.isPicked = isPicked;
            gameObject.SetActive(isActive);
            GetComponent<IUsableEntity>().canUse = true;
        }

        public void DisableItem(bool isActive, bool isPicked = true, bool canInteract = false)
        {
            this.canInteract = canInteract;
            this.isPicked = isPicked;
            gameObject.SetActive(isActive);
            GetComponent<IUsableEntity>().canUse = false;
        }

        //[ServerCallback]
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!canInteract && isPicked)
                return;
            if (collider.GetComponent<KB_PlayerController>())
            {
                inventoryHandler = collider.GetComponent<KB_PlayerController>().inventoryHandler;
                inventoryHandler.TryPickup(pickupManager.GetItemContainer());
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (!canInteract && isPicked)
                return;
            if (collider.GetComponent<KB_PlayerController>())
            {
                inventoryHandler = collider.GetComponent<KB_PlayerController>().inventoryHandler;
                inventoryHandler.RemovePickup(pickupManager.GetItemContainer());
            }
        }
    }
}


