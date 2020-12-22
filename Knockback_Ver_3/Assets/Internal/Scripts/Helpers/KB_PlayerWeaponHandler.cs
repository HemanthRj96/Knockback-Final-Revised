using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    public class KB_PlayerWeaponHandler
    {
        public KB_PlayerWeaponHandler() { }
        public KB_PlayerWeaponHandler(KB_PlayerController controlledActor) => this.controlledActor = controlledActor;

        private KB_PlayerController controlledActor;
        private GameObject weapon { get { return controlledActor.cachedWeaponSlot.GetChild(0).gameObject; } }
        private GameObject cachedWeapon { get; set; }


        public void UseWeapon(bool value)
        {
            if (!value)
                return;
            if (controlledActor.cachedWeaponSlot.childCount == 0)
                return;

            if (cachedWeapon == null || cachedWeapon != weapon)
            {
                if (weapon != null && weapon.activeInHierarchy)
                {
                    cachedWeapon = weapon;
                    Use(cachedWeapon.GetComponent<IUsableEntity>());
                }
                return;
            }
            else
                Use(cachedWeapon.GetComponent<IUsableEntity>());
        }

        private void Use(IUsableEntity item) { item.UseItem(controlledActor.gameObject); }
    }
}