using UnityEngine;
using Knockback.Controllers;
using Knockback.Handlers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    public class KB_PlayerSlotRotation
    {
        public KB_PlayerSlotRotation() { }
        public KB_PlayerSlotRotation(KB_PlayerController controlledActor) => this.controlledActor = controlledActor;

        private KB_PlayerController controlledActor = null;
        private KB_CameraController cameraController = null;

        private KB_InputSettings inputSettings { get { return controlledActor.inputSettings; } }

        private float rotationInDegrees = 0;
        private bool useAlternateInput = false;

        public Quaternion GetCalculatedRotation()
        {
            Vector2 difference = Vector2.zero;
            // Do this if you're running the game in debug mode
            Vector2 playerPos = controlledActor.transform.position;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Quaternion targetRotation = Quaternion.identity;

            if (cameraController == null)
                if (TryGettingCamera())
                    useAlternateInput = true;

            if (inputSettings.GetInputType() == Utility.InputType.MouseAndKeyboard && !useAlternateInput)
                difference = playerPos.GetDirectionOfVector(mousePos);
            else
                difference = new Vector2(inputSettings.MovementInput().x, inputSettings.MovementInput().y);

            if (difference != Vector2.zero)
                rotationInDegrees = difference.GetAngleOfRotationFromDirection();

            targetRotation = Quaternion.Euler(0, 0, rotationInDegrees);

            cameraController?.AddAimOffset(difference);

            if (rotationInDegrees > 90 || rotationInDegrees < -90)
            {
                controlledActor.m_cachedSpriteRenderer.flipX = true;
                targetRotation = Quaternion.Euler(180, 0, -rotationInDegrees);
            }
            else
                controlledActor.m_cachedSpriteRenderer.flipX = false;

            return targetRotation;
        }

        private bool TryGettingCamera() => KB_ReferenceHandler.GetReference("MainCameraController", out cameraController);

    }
}