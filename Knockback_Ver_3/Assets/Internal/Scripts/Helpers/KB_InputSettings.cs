using UnityEngine;
using Knockback.Utility;
using UnityStandardAssets.CrossPlatformInput;

namespace Knockback.Helpers
{
    [System.Serializable]
    public class KB_InputSettings
    {
        public string movementXInput;
        public string movementYInput;
        public string jumpInput;
        public string dashInput;
        public string fireInput;
        public string interactInput;
        public string cancelInput;
        public string readyInput;
        [SerializeField] private InputType inputType = InputType.Touch;

        [Range(0.6f, 1)]
        public float joystickDeadzone = 0.8f;

        public Vector2 MovementInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return new Vector2(Input.GetAxisRaw(movementXInput), Input.GetAxisRaw(movementYInput));
            else
                return new Vector2(CrossPlatformInputManager.GetAxisRaw(movementXInput), CrossPlatformInputManager.GetAxisRaw(movementYInput));
        }

        public bool JumpInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(jumpInput);
            else
                return CrossPlatformInputManager.GetButtonDown(jumpInput);
        }

        public bool DashInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(dashInput);
            else
                return CrossPlatformInputManager.GetButtonDown(dashInput);
        }

        public bool FireInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKey(fireInput);
            else
                return CrossPlatformInputManager.GetButton(fireInput);
        }

        public bool InteractInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(interactInput);
            else
                return CrossPlatformInputManager.GetButtonDown(interactInput);
        }

        public bool CancelInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(cancelInput);
            else
                return CrossPlatformInputManager.GetButtonDown(cancelInput);
        }

        public bool ReadyInput()
        {
            if (inputType == InputType.MouseAndKeyboard)
                return Input.GetKeyDown(readyInput);
            else
                return CrossPlatformInputManager.GetButtonDown(readyInput);
        }

        public InputType GetInputType() => inputType;

        public void SetInputType(InputType inputType) => this.inputType = inputType;
    }
}