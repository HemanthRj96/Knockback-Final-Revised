using UnityEngine;
using Knockback.Controllers;

namespace Knockback.Core
{
    public abstract class KB_GenericItemPickupCore : MonoBehaviour
    {
        protected virtual void UIUpdate(bool state) { }
        protected virtual void OnStartPickup() { }
        protected virtual void OnStopPickup() { }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>() != null)
            {
                UIUpdate(true);
                OnStartPickup();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>() != null)
            {
                UIUpdate(false);
                OnStopPickup();
            }
        }        
    }
}