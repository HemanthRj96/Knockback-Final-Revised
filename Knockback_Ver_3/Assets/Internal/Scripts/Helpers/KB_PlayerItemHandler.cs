using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    public class KB_PlayerItemHandler
    {
        public KB_PlayerItemHandler() { }
        public KB_PlayerItemHandler(KB_PlayerController controlledActor) => this.controlledActor = controlledActor;

        private KB_PlayerController controlledActor;

        private GameObject item = null;


        public void UseWeapon(bool value)
        {
            if (!value)
                return;
            if (controlledActor.cachedWeaponSlot.childCount == 0)
                return;

           // Get the item from the inventory and use it
        }

        private void Use(IUsableEntity item) { item.UseItem(controlledActor.gameObject); }
    }
}