using UnityEngine;
using Knockback.Controllers;

namespace Knockback.Core
{
    public abstract class KB_MiscItemPickupCore : MonoBehaviour
    {
        protected virtual void UIUpdate(bool state) { }
        protected virtual void OnStartPickup() { }
        protected virtual void OnStopPickup() { }
        protected virtual void SetIsPickedUp(bool state) => isPickedUp = state;

        private bool isPickedUp = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isPickedUp)
                return;
            if (collision.GetComponent<KB_PlayerController>() != null)
            {
                UIUpdate(true);
                OnStartPickup();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isPickedUp)
                return;
            if (collision.GetComponent<KB_PlayerController>() != null)
            {
                UIUpdate(false);
                OnStopPickup();
            }
        }        
    }
}