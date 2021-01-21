using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    public class KB_ItemInteractor : MonoBehaviour
    {
        [Header("Default backend settings")]
        [Space]
        [SerializeField] private GameObject iconPrefab;

        private bool isPicked => transform.parent != null;
        private bool canInteract = true;
        private KB_InventoryHandler bufferedInventoryHandler = null;
        private KB_ItemContainer itemContainer = new KB_ItemContainer();


        private void Awake() => InitializeContainer();

        private void InitializeContainer()
        {
            if (iconPrefab == null)
            {
                Debug.LogError("UI HANDLER NOT SETUP");
                canInteract = false;
                return;
            }
            itemContainer.SetupContainer(this, gameObject, iconPrefab);
        }

        public void EnableObject() => gameObject.SetActive(true);

        public void DisableObject() => gameObject.SetActive(false);

        public void SetUsability(bool canUse) => gameObject.GetComponent<IUsableEntity>().canUse = canUse;

        public void SetInteractability(bool canInteract) => this.canInteract = canInteract;

        public KB_ItemContainer GetItemContainer() => itemContainer;


        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!canInteract && isPicked)
                return;
            bufferedInventoryHandler = null;
            bufferedInventoryHandler = collider.GetComponent<KB_PlayerController>()?.inventoryHandler;
            if (bufferedInventoryHandler == null)
                return;

            bufferedInventoryHandler.TryPickup(itemContainer);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (!canInteract && isPicked)
                return;
            bufferedInventoryHandler = null;
            bufferedInventoryHandler = collider.GetComponent<KB_PlayerController>()?.inventoryHandler;
            if (bufferedInventoryHandler == null)
                return;

            bufferedInventoryHandler.RemovePickup(itemContainer);
        }
    }
}