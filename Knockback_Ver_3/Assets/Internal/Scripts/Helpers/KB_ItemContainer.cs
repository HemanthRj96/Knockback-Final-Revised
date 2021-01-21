using UnityEngine;

namespace Knockback.Helpers
{
    public class KB_ItemContainer
    {
        private GameObject item = null;
        private GameObject iconPrefab = null;
        private GameObject itemUser = null;
        private KB_ItemInteractor pickupManager = null;

        public void SetupContainer(KB_ItemInteractor pickupManager, GameObject item, GameObject iconPrefab)
        {
            this.pickupManager = pickupManager;
            this.item = item;
            this.iconPrefab = iconPrefab;
        }

        public GameObject GetItem() => item;
        public GameObject GetIconPrefab() => iconPrefab;
        public KB_ItemInteractor GetPickupManager() => pickupManager;
        public GameObject GetItemUser() => itemUser;
        public void SetItemUser(GameObject itemUser) => this.itemUser = itemUser;

    }
}