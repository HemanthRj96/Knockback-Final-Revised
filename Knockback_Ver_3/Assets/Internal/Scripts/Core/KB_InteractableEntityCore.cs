using UnityEngine;
using Knockback.Handlers;
using Knockback.Controllers;
using UnityEngine.UI;

namespace Knockback.Core
{
    //todo: More robust implementation for advanced mechanics - KB_InteractableEntityCore
    // This script is in a very basic form


    public class KB_InteractableEntityCore : MonoBehaviour
    {
        [Header("Broadcaster backend settings")]
        [Space]
        [SerializeField] private int m_entityId = 0;
        // Get this using reference handler
        [SerializeField] private GameObject m_targetGUIGameObject;
        [SerializeField] private string m_referenceHandlerString;
        [SerializeField] private string m_eventIdentifierTag;
        [SerializeField] private bool m_shouldCooldown = false;

        private bool m_canUse = false;
        private bool m_isActive = false;

        private void Awake()
        {
            if (KB_ReferenceHandler.GetReference(m_referenceHandlerString, out m_targetGUIGameObject))
                m_targetGUIGameObject.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            m_isActive = !m_isActive;
            if (m_isActive)
                StartUse();
            else
                StopUse();
        }

        private void StartUse()
        {
            if (!m_canUse)
                return;
            KB_EventHandler.Invoke(m_eventIdentifierTag, true, gameObject);
        }

        private void StopUse()
        {
            if (!m_canUse)
                return;
            KB_EventHandler.Invoke(m_eventIdentifierTag, false, gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>())
            {
                m_targetGUIGameObject.SetActive(true);
                m_canUse = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<KB_PlayerController>())
            {
                m_targetGUIGameObject.SetActive(false);
                m_canUse = false;
            }
        }
    }
}