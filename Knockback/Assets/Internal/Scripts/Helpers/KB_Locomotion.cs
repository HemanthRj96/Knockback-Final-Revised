using Knockback.Controllers;
using System.Collections;
using UnityEngine;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class implements all the locomotion logic for the player
    /// </summary>
    [System.Serializable]
    public class KB_Locomotion
    {
        //** --CONSTRUCTORS--

        public KB_Locomotion() { }

        public KB_Locomotion(KB_PlayerController controlledActor)
        {
            this.m_controlledActor = controlledActor;
            m_cachedRigidbody = controlledActor.m_cachedRigidbody;
            m_cachedSpriteRenderer = controlledActor.m_cachedSpriteRenderer;
        }

        //** --ATTRIBUTES--
        //** --PRIVATE ATTRIBUTES--

        private KB_PlayerController m_controlledActor = null;
        private Rigidbody2D m_cachedRigidbody = null;
        private SpriteRenderer m_cachedSpriteRenderer = null;
        private bool m_isGrounded = true;
        private bool m_rightOrLeft = true;
        private bool m_canDash = true;
        private bool m_isDashing = false;
        private bool m_isGroundCheckerRunning = false;
        private const float _JUMP_MULTIPLIER = 5;

        //** --PRIVATE REFERENCES--

        private KB_PlayerBackendSettings m_playerSettings { get { return m_controlledActor?.playerSettings; } }
        private KB_InputSettings m_inputSettings { get { return m_controlledActor?.inputSettings; } }

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to move the player in the X axis
        /// </summary>
        /// <param name="axisValue">Float axis value</param>
        public void Move(float axisValue)
        {
            if (!m_isGroundCheckerRunning)
                m_controlledActor.StartCoroutine(GroundChecker());
            if (m_isDashing)
                return;
            if (!CheckJoystickThreshold(axisValue))
                return;

            float airControl = m_isGrounded ? 1 : m_playerSettings.m_airControl;            
            UpdateDirection(axisValue);
            m_controlledActor.transform.position += ((new Vector3(axisValue * airControl, 0, 0)) * m_playerSettings.m_moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Method to jump the player in Y axis
        /// </summary>
        /// <param name="jumpInput">Bool input value</param>
        public void Jump(bool jumpInput)
        {
            if (!m_isGroundCheckerRunning)
                m_controlledActor.StartCoroutine(GroundChecker());
            if (!m_isGrounded || m_isDashing || !jumpInput)
                return;

            float exitSpeed = m_playerSettings.m_jumpForce * _JUMP_MULTIPLIER;
            m_cachedRigidbody.velocity = new Vector2(0, exitSpeed);
        }

        /// <summary>
        /// Method to dash the player in the X axis
        /// </summary>
        /// <param name="dashInput"></param>
        public void Dash(bool dashInput)
        {
            if (!m_isGroundCheckerRunning)
                m_controlledActor.StartCoroutine(GroundChecker());
            if (!m_canDash || !dashInput)
                return;

            m_canDash = false;
            m_controlledActor.StartCoroutine(StartDash(m_playerSettings));
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Returns true if the axisValue is greater than the joystick threshold value
        /// </summary>
        /// <param name="axisValue">Axis value</param>
        /// <returns></returns>
        private bool CheckJoystickThreshold(float axisValue) => Mathf.Abs(axisValue) > m_inputSettings.m_joystickDeadzone;

        /// <summary>
        /// Updates the direction of the player i.e, left or right
        /// </summary>
        /// <param name="xAxis">X axis value</param>
        private void UpdateDirection(float xAxis) => m_rightOrLeft = xAxis > 0 ? true : (xAxis < 0 ? false : m_rightOrLeft);

        /// <summary>
        /// GroundChecker coroutine that update the value of isGrounded attribute
        /// </summary>
        private IEnumerator GroundChecker()
        {
            m_isGroundCheckerRunning = true;

            Vector2 boxSize = new Vector2(m_cachedSpriteRenderer.bounds.size.x - 0.2f, 0.01f);
            Vector3 offset =  new Vector3(0f, m_cachedSpriteRenderer.bounds.extents.y + boxSize.y, 0f);            

            while (m_controlledActor.gameObject.activeInHierarchy)
            {
                Vector3 origin = m_controlledActor.transform.position - offset;
                RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, Vector2.down, boxSize.y, m_playerSettings.m_groundCheckerLayerMask);
                m_isGrounded = hit.collider != null;
                yield return null;
            }

            m_isGroundCheckerRunning = false;
            yield return null;
        }

        /// <summary>
        /// Courtine to dash the player in x axis
        /// </summary>
        /// <param name="playerSettings">Backend settings</param>
        /// <returns></returns>
        private IEnumerator StartDash(KB_PlayerBackendSettings playerSettings)
        {
            m_isDashing = true;

            int direction = m_rightOrLeft ? 1 : -1;
            float elapsedTime = 0;
            float totalDashDistance = 0;
            Vector2 boxSize = new Vector2(playerSettings.m_dashingDistance, m_cachedSpriteRenderer.bounds.size.y - 0.2f);
            Vector2 initialPos = m_controlledActor.transform.position;
            Vector2 offset = new Vector3(m_cachedSpriteRenderer.bounds.extents.x, 0);
            Vector2 origin = initialPos - offset;
            LayerMask playerIgnoreMask = 1 << 8;

            RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0, new Vector2(direction, 0), playerSettings.m_dashingDistance, ~playerIgnoreMask);

            if (hit.collider)
            {
                offset = new Vector2(direction * m_cachedSpriteRenderer.bounds.extents.x, 0);
                totalDashDistance = Vector2.Distance(m_controlledActor.transform.position, hit.point - offset);
            }
            else
                totalDashDistance = playerSettings.m_dashingDistance;

            m_controlledActor.transform.position += new Vector3(totalDashDistance * direction, 0, 0);

            m_isDashing = false;

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;
                return elapsedTime > playerSettings.m_dashingCooldown;
            });

            m_canDash = true;
        }
    }
}