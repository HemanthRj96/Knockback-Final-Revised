using UnityEngine;
using Knockback.Handlers;
using EZCameraShake;
using Knockback.Scriptables;

namespace Knockback.Controllers
{
    public class KB_CameraController : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Camera controller backend settings")]
        [Space]
        [SerializeField] KB_CameraData m_cameraData = null;
        [SerializeField] Vector3 m_cameraRestingPosition;

        //** --PUBLIC ATTRIBUTES--

        public GameObject localTarget = null;

        //** --PRIVATE ATTRIBUTES--

        private bool m_isCameraFollowing => localTarget != null;
        private Vector3 m_aimOffset;
        private Vector3 m_targetPosition;
        private Vector3 m_smoothAimOffset;
        private bool m_canUse = false;
        private Camera m_mainCamera = null;
        private const string _REFERENCE_TAG = "MainCameraController";

        //** --METHODS--
        //** --PRIVATE METHODS--

        /// <summary>
        /// Camera bootstrapping happens  here
        /// </summary>
        private void Awake() => CameraBootstrap();

        /// <summary>
        /// Camera position and offset update happens here
        /// </summary>
        private void FixedUpdate()
        {
            if (m_canUse)
            {
                if (m_isCameraFollowing)
                {
                    UpdateCameraPositionAndAimOffset();
                    ClampCameraBounds();
                }
                else
                {
                    // Do something here
                }
            }
        }

        /// <summary>
        /// Bootstrapper functions
        /// </summary>
        private void CameraBootstrap()
        {
            if (m_cameraData == null)
                return;

            KB_ReferenceHandler.Add(this, _REFERENCE_TAG);
            transform.position += new Vector3(0, 0, m_cameraData.cameraRestingZOffset);
            m_mainCamera = GetComponentInChildren<Camera>();
            m_mainCamera.orthographicSize = m_cameraData.cameraFOV;
            SetToDefaultPosition();
            m_canUse = true;
        }

        /// <summary>
        /// Returns the target objects position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetTargetPosition()
        {
            float displacement = Vector2.Distance(transform.position, localTarget.transform.position);
            float lerpControlVariable = displacement * m_cameraData.cameraDampingFactor.magnitude * Time.deltaTime;

            return new Vector3
                    (
                        Mathf.Lerp
                            (
                                transform.position.x,
                                localTarget.transform.position.x,
                                lerpControlVariable
                            ),
                        Mathf.Lerp
                            (
                                transform.position.y,
                                localTarget.transform.position.y,
                                lerpControlVariable
                            ),
                        transform.position.z
                    );
        }

        /// <summary>
        /// Updates the camera position and offset
        /// </summary>
        private void UpdateCameraPositionAndAimOffset()
        {
            m_targetPosition = GetTargetPosition();
            m_aimOffset.Normalize();
            m_smoothAimOffset = new Vector3
                (
                    Mathf.Lerp
                        (
                            m_smoothAimOffset.x,
                            m_aimOffset.x,
                            m_cameraData.offsetDampingFactor.x * Time.deltaTime
                        ),
                    Mathf.Lerp
                        (
                            m_smoothAimOffset.y,
                            m_aimOffset.y,
                            m_cameraData.offsetDampingFactor.y * Time.deltaTime
                        ),
                    transform.position.z
                );

            transform.position = new Vector3
                (
                    m_targetPosition.x + (m_smoothAimOffset.x * m_cameraData.offsetScaler.x / 10),
                    m_targetPosition.y + (m_smoothAimOffset.y * m_cameraData.offsetScaler.y / 10),
                    transform.position.z
                );
        }

        /// <summary>
        /// Clamps the maximum limits of the camera
        /// </summary>
        private void ClampCameraBounds()
        {
            transform.position = new Vector3
                    (
                        Mathf.Clamp(transform.position.x, m_cameraData.minimumBounds.x, m_cameraData.maximumBounds.x),
                        Mathf.Clamp(transform.position.y, m_cameraData.minimumBounds.y, m_cameraData.maximumBounds.y),
                        transform.position.z
                    );
        }

        /// <summary>
        /// Sets the camera's position to the default position
        /// </summary>
        private void SetToDefaultPosition() => transform.position = m_cameraRestingPosition;

        /// <summary>
        /// Resets the camera to the defgault position
        /// </summary>
        private void ResetCamera()
        {
            SetToDefaultPosition();
            localTarget = null;
        }


        //** --PUBLIC METHODS--

        /// <summary>
        /// Returns the main camera
        /// </summary>
        public Camera GetCamera() => m_mainCamera;

        /// <summary>
        /// Adds aim offset to the camera
        /// </summary>
        public void AddAimOffset(Vector2 offset) => m_aimOffset = new Vector3(offset.x, offset.y, transform.position.z);

        /// <summary>
        /// Sets the local target the camera should follow
        /// </summary>
        public void SetLocalTarget(GameObject localTarget) => this.localTarget = localTarget;

        /// <summary>
        /// Shakes the camera with a magnitude
        /// </summary>
        /// <param name="magnitude">Magnitude of the shake</param>
        public void ShakeCameraWithMagnitude(float magnitude) => CameraShaker.Instance.ShakeOnce(magnitude, m_cameraData.roughness, m_cameraData.fadeInTime, m_cameraData.fadeOutTime);

        /// <summary>
        /// Removes the local target and resets the camera positions
        /// </summary>
        public void RemoveLocalTarget() => ResetCamera();

        /// <summary>
        /// Allows to set the camera resting position externally
        /// </summary>
        public void SetTheCameraRestingPosition(Vector3 restingPosition) => m_cameraRestingPosition = restingPosition;
    }
}
