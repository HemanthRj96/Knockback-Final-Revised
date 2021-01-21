using UnityEngine;
using Knockback.Controllers;
using Knockback.Handlers;

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
            Quaternion targetRotation = Quaternion.identity;

            if (cameraController == null)
                if (TryGettingCamera())
                    useAlternateInput = true;

            if (inputSettings.GetInputType() == Utility.InputType.MouseAndKeyboard && !useAlternateInput)
            {
                difference = cameraController.GetCamera().ScreenToWorldPoint(Input.mousePosition) - controlledActor.transform.position;
                difference.Normalize();
            }
            else
            {
                difference = new Vector2(inputSettings.MovementInput().x, inputSettings.MovementInput().y);
            }

            if (difference != Vector2.zero)
                rotationInDegrees = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            targetRotation = Quaternion.Euler(0, 0, rotationInDegrees);

            cameraController?.AddAimOffset(difference);

            if (rotationInDegrees > 90 || rotationInDegrees < -90)
            {
                controlledActor.cachedSpriteRenderer.flipX = true;
                targetRotation = Quaternion.Euler(180, 0, -rotationInDegrees);
            }
            else
                controlledActor.cachedSpriteRenderer.flipX = false;

            return targetRotation;
        }

        private bool TryGettingCamera() => KB_ReferenceHandler.GetReference("MainCameraController", out cameraController);

    }
}