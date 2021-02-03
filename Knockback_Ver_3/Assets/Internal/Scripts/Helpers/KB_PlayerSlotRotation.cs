using UnityEngine;
using Knockback.Controllers;
using Knockback.Handlers;
using Knockback.Utility;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class implements the player weapon slot rotation
    /// </summary>
    public class KB_PlayerSlotRotation
    {
        //** --CONSTRUCTORS--

        public KB_PlayerSlotRotation() { }
        public KB_PlayerSlotRotation(KB_PlayerController controlledActor) => this.m_controlledActor = controlledActor;

        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES--

        private KB_PlayerController m_controlledActor = null;
        private KB_CameraController m_cameraController = null;
        private float m_rotationInDegrees = 0;
        private bool m_useAlternateInput = false;

        //** --PRIVATE REFERENCES--

        private KB_InputSettings m_inputSettings => m_controlledActor.inputSettings;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Returns a quaternion for the weapon slot based on the input
        /// </summary>
        /// <returns></returns>
        public Quaternion GetCalculatedRotation()
        {
            Vector2 difference = Vector2.zero;
            // Do this if you're running the game in debug mode
            Vector2 playerPos = m_controlledActor.transform.position;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Quaternion targetRotation = Quaternion.identity;
            if (m_cameraController == null)
                if (TryGettingCamera())
                    m_useAlternateInput = true;
            if (m_inputSettings.GetInputType() == Utility.InputType.MouseAndKeyboard && !m_useAlternateInput)
                difference = playerPos.GetDirectionOfVector(mousePos);
            else
                difference = new Vector2(m_inputSettings.MovementInput().x, m_inputSettings.MovementInput().y);
            if (difference != Vector2.zero)
                m_rotationInDegrees = difference.GetAngleOfRotationFromDirection();
            targetRotation = Quaternion.Euler(0, 0, m_rotationInDegrees);
            m_cameraController?.AddAimOffset(difference);
            if (m_rotationInDegrees > 90 || m_rotationInDegrees < -90)
            {
                m_controlledActor.m_cachedSpriteRenderer.flipX = true;
                targetRotation = Quaternion.Euler(180, 0, -m_rotationInDegrees);
            }
            else
                m_controlledActor.m_cachedSpriteRenderer.flipX = false;
            return targetRotation;
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Method to intialize the main camera if any
        /// </summary>
        private bool TryGettingCamera() => KB_ReferenceHandler.GetReference("MainCameraController", out m_cameraController);

    }
}