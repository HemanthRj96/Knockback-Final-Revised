using Knockback.Controllers;
using System.Collections;
using UnityEngine;

namespace Knockback.Helpers
{
    [System.Serializable]
    public class KB_Locomotion
    {
        public KB_Locomotion() { }

        public KB_Locomotion(KB_PlayerController controlledActor)
        {
            this.controlledActor = controlledActor;
            cachedRigidbody = controlledActor.m_cachedRigidbody;
            cachedSpriteRenderer = controlledActor.m_cachedSpriteRenderer;
        }

        // Cached variables
        private KB_PlayerController controlledActor = null;
        private Rigidbody2D cachedRigidbody = null;
        private SpriteRenderer cachedSpriteRenderer = null;


        private const float _JUMP_MULTIPLIER = 5;
        private bool isGrounded = true;
        private bool rightOrLeft = true;
        private bool canDash = true;
        private bool isDashing = false;
        private bool isGroundCheckerRunning = false;

        //Reference variables
        private KB_PlayerBackendSettings playerSettings { get { return controlledActor?.playerSettings; } }
        private KB_InputSettings inputSettings { get { return controlledActor?.inputSettings; } }


        //****************************************************************************** Public functions

        public void Move(float axisValue)
        {
            if (!isGroundCheckerRunning)
                controlledActor.StartCoroutine(IsGrounded());
            if (isDashing)
                return;
            if (!CheckJoystickThreshold(axisValue))
                return;

            float airControl = isGrounded ? 1 : playerSettings.airControl;            
            UpdateDirection(axisValue);
            controlledActor.transform.position += ((new Vector3(axisValue * airControl, 0, 0)) * playerSettings.moveSpeed * Time.deltaTime);
        }

        public void Jump(bool jumpInput)
        {
            if (!isGroundCheckerRunning)
                controlledActor.StartCoroutine(IsGrounded());
            if (!isGrounded || isDashing || !jumpInput)
                return;

            float exitSpeed = playerSettings.jumpForce * _JUMP_MULTIPLIER;
            cachedRigidbody.velocity = new Vector2(0, exitSpeed);
        }

        public void Dash(bool dashInput)
        {
            if (!isGroundCheckerRunning)
                controlledActor.StartCoroutine(IsGrounded());
            if (!canDash || !dashInput)
                return;

            canDash = false;
            controlledActor.StartCoroutine(StartDash(playerSettings));
        }

        //****************************************************************************** Private functions


        private bool CheckJoystickThreshold(float axisValue)
        {
            return Mathf.Abs(axisValue) > inputSettings.joystickDeadzone;
        }

        private void UpdateDirection(float xAxis)
        {
            rightOrLeft = xAxis > 0 ? true : (xAxis < 0 ? false : rightOrLeft);
        }

        private IEnumerator IsGrounded()
        {
            isGroundCheckerRunning = true;

            Vector2 boxSize = new Vector2(cachedSpriteRenderer.bounds.size.x - 0.2f, 0.01f);
            Vector3 offset =  new Vector3(0f, cachedSpriteRenderer.bounds.extents.y + boxSize.y, 0f);            

            while (controlledActor.gameObject.activeInHierarchy)
            {
                Vector3 origin = controlledActor.transform.position - offset;
                RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, Vector2.down, boxSize.y, playerSettings.groundCheckerLayerMask);
                isGrounded = hit.collider != null;
                yield return null;
            }

            isGroundCheckerRunning = false;
            yield return null;
        }

        private IEnumerator StartDash(KB_PlayerBackendSettings playerSettings)
        {
            isDashing = true;

            int direction = rightOrLeft ? 1 : -1;
            float elapsedTime = 0;
            float totalDashDistance = 0;
            Vector2 boxSize = new Vector2(playerSettings.dashingDistance, cachedSpriteRenderer.bounds.size.y - 0.2f);
            Vector2 initialPos = controlledActor.transform.position;
            Vector2 offset = new Vector3(cachedSpriteRenderer.bounds.extents.x, 0);
            Vector2 origin = initialPos - offset;
            LayerMask playerIgnoreMask = 1 << 8;

            RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, new Vector2(direction, 0), playerSettings.dashingDistance, ~playerIgnoreMask);

            if (hit.collider)
            {
                offset = new Vector2(direction * cachedSpriteRenderer.bounds.extents.x, 0);
                totalDashDistance = Vector2.Distance(controlledActor.transform.position, hit.point - offset);
            }
            else
                totalDashDistance = playerSettings.dashingDistance;

            controlledActor.transform.position += new Vector3(totalDashDistance * direction, 0, 0);

            isDashing = false;

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;
                return elapsedTime > playerSettings.dashingCooldown;
            });

            canDash = true;
        }
    }
}