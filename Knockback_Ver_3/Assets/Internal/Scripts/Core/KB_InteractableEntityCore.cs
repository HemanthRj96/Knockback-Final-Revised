using UnityEngine;
using Knockback.Handlers;
using Knockback.Controllers;
using UnityEngine.UI;

namespace Knockback.Core
{
    //todo: More robust implementation for advanced mechanics
    // This script is in a very basic form
    public class KB_InteractableEntityCore : MonoBehaviour
    {
        [Header("Broadcaster backend settings")]
        [Space]
        [SerializeField] private int entityId = 0;
        // Get this using reference handler
        [SerializeField] private GameObject targetGUIGameObject;
        [SerializeField] private string referenceHandlerString;
        [SerializeField] private string eventIdentifierTag;
        [SerializeField] private bool shouldCooldown = false;

        private bool canUse = false;
        private bool isActive = false;

        private void Awake()
        {
            if (KB_ReferenceHandler.GetReference(referenceHandlerString, out targetGUIGameObject))
                targetGUIGameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            isActive = !isActive;
            if (isActive)
                StartUse();
            else
                StopUse();
        }

        private void StartUse()
        {
            if (!canUse)
                return;
            KB_EventHandler.Invoke(eventIdentifierTag, true, gameObject);
        }

        private void StopUse()
        {
            if (!canUse)
                return;
            KB_EventHandler.Invoke(eventIdentifierTag, false, gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>())
            {
                targetGUIGameObject.SetActive(true);
                canUse = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>())
            {
                targetGUIGameObject.SetActive(false);
                canUse = false;
            }
        }
    }
}