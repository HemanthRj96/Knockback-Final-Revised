using UnityEngine;
using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine.UI;
using Knockback.Helpers;

namespace Knockback.Handlers
{
    /// <summary>
    /// Use this class to manage multiple canvas groups in a single scene which are controlled by buttons
    /// </summary>
    public class KB_CanvasGroupHandler : MonoBehaviour
    {
        //** --INTERNAL CLASSES--

        [System.Serializable]
        internal class _CanvasGroups
        {
            public bool m_activeByDefault = false;
            public GameObject m_canvasGroup = null;
        }

        [System.Serializable]
        internal class _ButtonLookup
        {
            public Button m_targetButton = null;
            public string m_buttonName = null;
            public GameObject m_parentCanvasGroup = null;
            public bool m_shouldSelfDeactivate = true;
            public GameObject m_targetCanvasGroup = null;
            public GameObject m_targetAction = null;
        }

        //** --SERIALIZED ATTRIBUTES--

        [Header("UI handler backend settings")]
        [Space]
        [SerializeField] private bool m_activateExternally = false;
        [SerializeField] private string m_canvasGroupHandlerName = "_CanvasGroupHandler_";
        [SerializeField] private List<_CanvasGroups> m_canvasGroupList = new List<_CanvasGroups>();
        [SerializeField] private List<_ButtonLookup> m_buttonList = new List<_ButtonLookup>();


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to prevent the canvas to be bootstrapped externally
        /// </summary>
        public void BootstrapExternally()
        {
            if (m_activateExternally)
                CanvasBootstrapper();
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Calls camera bootstrapper on awake
        /// </summary>
        private void Awake()
        {
            if (m_activateExternally)
                DeactivateAllCanvas();
            else
                CanvasBootstrapper();
        }

        /// <summary>
        /// Method to deactivate all the canvas groups
        /// </summary>
        private void DeactivateAllCanvas()
        {
            foreach (var canvas in m_canvasGroupList)
                DeactivateCanvasGroup(canvas.m_canvasGroup);
        }

        /// <summary>
        /// Remove the event from event handler
        /// </summary>
        private void OnDestroy() => KB_EventHandler.RemoveEvent(m_canvasGroupHandlerName);

        /// <summary>
        /// Activate canvas groups that should be active by default
        /// </summary>
        private void CanvasBootstrapper()
        {
            KB_EventHandler.AddEvent(m_canvasGroupHandlerName, CanvasUpdate);
            ButtonBootstrapper();

            foreach (var canvasGroup in m_canvasGroupList)
            {
                if (canvasGroup.m_activeByDefault)
                    canvasGroup.m_canvasGroup.SetActive(true);
                else
                    canvasGroup.m_canvasGroup.SetActive(false);
            }
        }

        /// <summary>
        /// Dynamically binds all the button to the correct method
        /// </summary>
        private void ButtonBootstrapper()
        {
            KB_CanvasGroupButtonBinder tempBinder = null;
            foreach (var buttonLookup in m_buttonList)
            {
                tempBinder = buttonLookup.m_targetButton.gameObject.AddComponent<KB_CanvasGroupButtonBinder>();
                tempBinder.m_targetEventTag = m_canvasGroupHandlerName;
                tempBinder.m_buttonName = buttonLookup.m_buttonName;
                buttonLookup.m_targetButton.onClick.AddListener(tempBinder.OnButtonClick);
            }
        }

        /// <summary>
        /// Called by the buttons whenenver they are clicked
        /// </summary>
        /// <param name="message">Message has string as data</param>
        private void CanvasUpdate(IMessage message)
        {
            string buttonName = message.data as string;
            _ButtonLookup buttonLookup = FindButton(buttonName);

            if (buttonLookup == null)
                return;

            if (buttonLookup.m_shouldSelfDeactivate)
                DeactivateCanvasGroup(buttonLookup.m_parentCanvasGroup);
            ActivateCanvasGroup(buttonLookup.m_targetCanvasGroup);
            if (buttonLookup.m_targetAction != null)
            {
                if (!buttonLookup.m_targetAction.activeSelf)
                {
                    GameObject actionInstance = Instantiate(buttonLookup.m_targetAction);
                    actionInstance.GetComponent<IUIAction>().DoAction();
                }
                else
                    buttonLookup.m_targetAction.GetComponent<IUIAction>().DoAction();
            }
        }

        /// <summary>
        /// Helper method to find button from the button list
        /// </summary>
        /// <param name="buttonName">Name of the button</param>
        private _ButtonLookup FindButton(string buttonName)
        {
            foreach (var temp in m_buttonList)
                if (temp.m_buttonName == buttonName)
                    return temp;
            return null;
        }

        /// <summary>
        /// Method to activate the target canvas group if it exists
        /// </summary>
        /// <param name="tag">Name of the canvas group</param>
        private void ActivateCanvasGroup(GameObject canvasGroup)
        {
            if (canvasGroup == null)
                return;
            canvasGroup.SetActive(true);
        }

        /// <summary>
        /// Method to deactivate the canvas group if it exists
        /// </summary>
        /// <param name="tag">Name of the canvas group</param>
        private void DeactivateCanvasGroup(GameObject canvasGroup)
        {
            if (canvasGroup == null)
                return;
            canvasGroup?.SetActive(false);
        }
    }
}