using UnityEngine;
using Knockback.Utility;
using UnityStandardAssets.CrossPlatformInput;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class handles all the inputs from the user
    /// </summary>
    [System.Serializable]
    public class KB_InputSettings
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private InputType m_inputType = InputType.Touch;

        //** --PUBLIC ATTRIBUTES--

        public string m_movementXInput;
        public string m_movementYInput;
        public string m_jumpInput;
        public string m_dashInput;
        public string m_fireInput;
        public string m_interactInput;
        public string m_cancelInput;
        public string m_readyInput;
        [Range(0.6f, 1)] public float m_joystickDeadzone = 0.8f;


        //** --METHODS--
        //** --PUBLIC METHODS--

        public Vector2 MovementInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return new Vector2(Input.GetAxisRaw(m_movementXInput), Input.GetAxisRaw(m_movementYInput));
            else
                return new Vector2(CrossPlatformInputManager.GetAxisRaw(m_movementXInput), CrossPlatformInputManager.GetAxisRaw(m_movementYInput));
        }

        public bool JumpInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(m_jumpInput);
            else
                return CrossPlatformInputManager.GetButtonDown(m_jumpInput);
        }

        public bool DashInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(m_dashInput);
            else
                return CrossPlatformInputManager.GetButtonDown(m_dashInput);
        }

        public bool FireInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKey(m_fireInput);
            else
                return CrossPlatformInputManager.GetButton(m_fireInput);
        }

        public bool InteractInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(m_interactInput);
            else
                return CrossPlatformInputManager.GetButtonDown(m_interactInput);
        }

        public bool CancelInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(m_cancelInput);
            else
                return CrossPlatformInputManager.GetButtonDown(m_cancelInput);
        }

        public bool ReadyInput()
        {
            if (m_inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(m_readyInput);
            else
                return CrossPlatformInputManager.GetButtonDown(m_readyInput);
        }

        /// <summary>
        /// Returns the input type
        /// </summary>
        public InputType GetInputType() => m_inputType;

        /// <summary>
        /// Method to set the input type
        /// </summary>
        /// <param name="inputType">Target input type</param>
        public void SetInputType(InputType inputType) => this.m_inputType = inputType;
    }
}