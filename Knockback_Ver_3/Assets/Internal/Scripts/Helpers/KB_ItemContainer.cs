using UnityEngine;
using System.Collections;

namespace Knockback.Helpers
{
    public class KB_ItemContainer
    {
        private GameObject item = null;
        private GameObject iconPrefab = null;
        private KB_PickupManager pickupManager = null;

        public void SetupContainer(KB_PickupManager pickupManager, GameObject item, GameObject iconPrefab)
        {
            this.pickupManager = pickupManager;
            this.item = item;
            this.iconPrefab = iconPrefab;
        }

        public GameObject GetItem() => item;
        public GameObject GetIconPrefab() => iconPrefab;
        public KB_PickupManager GetPickupManager() => pickupManager;
    }
}