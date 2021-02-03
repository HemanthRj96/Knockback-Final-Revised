using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Knockback.Handlers
{
    /// <summary>
    /// Class used for handling joystick position and scaling and savves it offline
    /// </summary>
    public class KB_JoystickSaveHandler : MonoBehaviour
    {
        //todo: Commenting

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private GameObject m_joystick = null;
        [SerializeField] private GameObject m_shootButton = null;
        [SerializeField] private GameObject m_jumpButton = null;
        [SerializeField] private GameObject m_dashButton = null;
        [SerializeField] private GameObject m_inventorySlotDock = null;
        [SerializeField] private Slider m_slider;
        [SerializeField] private float m_maxScaleJoystickButton;
        [SerializeField] private float m_maxScaleJumpButton;
        [SerializeField] private float m_maxScaleAimButton;
        [SerializeField] private float m_maxScaleDashButton;
        [SerializeField] private float m_maxScaleInventorySlotDock;

        //** --PRIVATE ATTRIBUTES--

        private bool m_isButtonSelected = false;
        private int m_selectedButton = -1;
        private int m_previousButton;
        private KB_DatabaseHandler m_dataBaseReference = new KB_DatabaseHandler();
        private Dictionary<int, Transform> m_UIButtonCollection = new Dictionary<int, Transform>();
        private Dictionary<int, float> m_maxScaleCollection = new Dictionary<int, float>();

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Select the button on button press
        /// </summary>
        /// <param name="buttonId"></param>
        public void OnButtonPress(int buttonId)
        {
            m_selectedButton = buttonId;
            m_isButtonSelected = !m_isButtonSelected;
        }

        /// <summary>
        ///  Update the selected button scale
        /// </summary>
        /// <param name="value"></param>
        public void OnSlide(float value)
        {
            float targetScale;
            if (m_selectedButton != -1)
            {
                m_previousButton = m_selectedButton;
                targetScale = Mathf.Clamp(value * m_maxScaleCollection[m_selectedButton], 0.5f, m_maxScaleCollection[m_selectedButton]);
                m_UIButtonCollection[m_selectedButton].transform.localScale = new Vector3(targetScale, targetScale, 0);
            }
        }

        /// <summary>
        /// To save the changes to the file offline
        /// </summary>
        public void SaveButton() => SaveSettings();

        //** --PRIVATE METHODS--

        /// <summary>
        /// Intialize ditcionaries and load data if it already exists
        /// </summary>
        private void Awake()
        {

            m_UIButtonCollection.Add(0, m_joystick.transform);
            m_UIButtonCollection.Add(1, m_shootButton.transform);
            m_UIButtonCollection.Add(2, m_jumpButton.transform);
            m_UIButtonCollection.Add(3, m_dashButton.transform);
            m_UIButtonCollection.Add(4, m_inventorySlotDock.transform);

            m_maxScaleCollection.Add(0, m_maxScaleJoystickButton);
            m_maxScaleCollection.Add(1, m_maxScaleJumpButton);
            m_maxScaleCollection.Add(2, m_maxScaleAimButton);
            m_maxScaleCollection.Add(3, m_maxScaleDashButton);
            m_maxScaleCollection.Add(4, m_maxScaleInventorySlotDock);

            // Load data if it exists
            if (LoadSettings())
            {
                m_dataBaseReference.GetJoystickData().CopyFromJoystickData(m_UIButtonCollection);
            }
        }

        /// <summary>
        /// Update any button changes
        /// </summary>
        private void Update()
        {
            if (m_previousButton != m_selectedButton) { m_slider.SetValueWithoutNotify(0); }

            if (m_isButtonSelected)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    m_UIButtonCollection[m_selectedButton].transform.position = touch.position;
                }
            }
            m_dataBaseReference.GetJoystickData().CopyToJoystickData(m_UIButtonCollection);
        }

        /// <summary>
        /// Returns true if the data was laoded false otherwise
        /// </summary>
        /// <returns></returns>
        private bool LoadSettings()
        {
            if (KB_DataPersistenceHandler.SaveExists(KB_DatabaseHandler.GetTargetDirectory()))
            {
                KB_DataPersistenceHandler.LoadData(KB_DatabaseHandler.GetTargetDirectory(), out m_dataBaseReference);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the settings to a configuration file offline
        /// </summary>
        private void SaveSettings() => KB_DataPersistenceHandler.SaveData(KB_DatabaseHandler.GetTargetDirectory(), m_dataBaseReference);
    }
}