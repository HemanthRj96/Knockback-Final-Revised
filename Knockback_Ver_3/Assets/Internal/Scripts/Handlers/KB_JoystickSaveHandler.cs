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

        public GameObject joystick = null;
        public GameObject shootButton = null;
        public GameObject jumpButton = null;
        public GameObject dashButton = null;
        public GameObject inventorySlotDock = null;
        public Slider slider;

        public float maxScaleJoystickButton;
        public float maxScaleJumpButton;
        public float maxScaleAimButton;
        public float maxScaleDashButton;
        public float maxScaleInventorySlotDock;

        private bool isButtonSelected = false;

        private int selectedButton = -1;
        private int previousButton;


        KB_DatabaseHandler dataBaseReference = new KB_DatabaseHandler();

        private Dictionary<int, Transform> UIButtonCollection = new Dictionary<int, Transform>();
        private Dictionary<int, float> maxScaleCollection = new Dictionary<int, float>();

        private void Awake()
        {

            UIButtonCollection.Add(0, joystick.transform);
            UIButtonCollection.Add(1, shootButton.transform);
            UIButtonCollection.Add(2, jumpButton.transform);
            UIButtonCollection.Add(3, dashButton.transform);
            UIButtonCollection.Add(4, inventorySlotDock.transform);

            maxScaleCollection.Add(0, maxScaleJoystickButton);
            maxScaleCollection.Add(1, maxScaleJumpButton);
            maxScaleCollection.Add(2, maxScaleAimButton);
            maxScaleCollection.Add(3, maxScaleDashButton);
            maxScaleCollection.Add(4, maxScaleInventorySlotDock);

            // Load data if it exists
            if (LoadSettings())
            {
                dataBaseReference.GetJoystickData().CopyFromJoystickData(UIButtonCollection);
            }
        }

        private void Update()
        {
            if (previousButton != selectedButton) { slider.SetValueWithoutNotify(0); }

            if (isButtonSelected)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    UIButtonCollection[selectedButton].transform.position = touch.position;
                }
            }
            dataBaseReference.GetJoystickData().CopyToJoystickData(UIButtonCollection);
        }

        public void OnButtonPress(int buttonTypes)
        {
            selectedButton = buttonTypes;
            isButtonSelected = !isButtonSelected;
        }

        public void OnSlide(float value)
        {
            float targetScale;
            if (selectedButton != -1)
            {
                previousButton = selectedButton;
                targetScale = Mathf.Clamp(value * maxScaleCollection[selectedButton], 0.5f, maxScaleCollection[selectedButton]);
                UIButtonCollection[selectedButton].transform.localScale = new Vector3(targetScale, targetScale, 0);
            }
        }

        public void SaveButton() { SaveSettings(); }

        private bool LoadSettings()
        {
            if (KB_DataPersistenceHandler.SaveExists(KB_DatabaseHandler.GetTargetDirectory()))
            {
                KB_DataPersistenceHandler.LoadData(KB_DatabaseHandler.GetTargetDirectory(), out dataBaseReference);
                return true;
            }
            return false;
        }

        private void SaveSettings()
        {
            dataBaseReference.GetPlayerData().SetValue(45);
            KB_DataPersistenceHandler.SaveData(KB_DatabaseHandler.GetTargetDirectory(), dataBaseReference);
        }
    }
}