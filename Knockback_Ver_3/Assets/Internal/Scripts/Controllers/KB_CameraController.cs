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
        [SerializeField] KB_CameraData cameraData = null;
        [SerializeField] Vector3 cameraRestingPosition;

        //** --PUBLIC ATTRIBUTES--

        public GameObject localTarget = null;

        //** --PRIVATE ATTRIBUTES--

        private bool isCameraFollowing => localTarget != null;
        private Vector3 aimOffset;
        private Vector3 targetPosition;
        private Vector3 smoothAimOffset;
        private bool canUse = false;
        private Camera mainCamera = null;
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
            if (canUse)
            {
                if (isCameraFollowing)
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
            if (cameraData == null)
                return;

            KB_ReferenceHandler.Add(this, _REFERENCE_TAG);
            transform.position += new Vector3(0, 0, cameraData.cameraRestingZOffset);
            mainCamera = GetComponentInChildren<Camera>();
            mainCamera.orthographicSize = cameraData.cameraFOV;
            SetToDefaultPosition();
            canUse = true;
        }

        /// <summary>
        /// Returns the target objects position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetTargetPosition()
        {
            float displacement = Vector2.Distance(transform.position, localTarget.transform.position);
            float lerpControlVariable = displacement * cameraData.cameraDampingFactor.magnitude * Time.deltaTime;

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
            targetPosition = GetTargetPosition();
            aimOffset.Normalize();
            smoothAimOffset = new Vector3
                (
                    Mathf.Lerp
                        (
                            smoothAimOffset.x,
                            aimOffset.x,
                            cameraData.offsetDampingFactor.x * Time.deltaTime
                        ),
                    Mathf.Lerp
                        (
                            smoothAimOffset.y,
                            aimOffset.y,
                            cameraData.offsetDampingFactor.y * Time.deltaTime
                        ),
                    transform.position.z
                );

            transform.position = new Vector3
                (
                    targetPosition.x + (smoothAimOffset.x * cameraData.offsetScaler.x / 10),
                    targetPosition.y + (smoothAimOffset.y * cameraData.offsetScaler.y / 10),
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
                        Mathf.Clamp(transform.position.x, cameraData.minimumBounds.x, cameraData.maximumBounds.x),
                        Mathf.Clamp(transform.position.y, cameraData.minimumBounds.y, cameraData.maximumBounds.y),
                        transform.position.z
                    );
        }

        /// <summary>
        /// Sets the camera's position to the default position
        /// </summary>
        private void SetToDefaultPosition() => transform.position = cameraRestingPosition;

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
        public Camera GetCamera() => mainCamera;

        /// <summary>
        /// Adds aim offset to the camera
        /// </summary>
        public void AddAimOffset(Vector2 offset) => aimOffset = new Vector3(offset.x, offset.y, transform.position.z);

        /// <summary>
        /// Sets the local target the camera should follow
        /// </summary>
        public void SetLocalTarget(GameObject localTarget) => this.localTarget = localTarget;

        /// <summary>
        /// Shakes the camera with a magnitude
        /// </summary>
        /// <param name="magnitude">Magnitude of the shake</param>
        public void ShakeCameraWithMagnitude(float magnitude) => CameraShaker.Instance.ShakeOnce(magnitude, cameraData.roughness, cameraData.fadeInTime, cameraData.fadeOutTime);

        /// <summary>
        /// Removes the local target and resets the camera positions
        /// </summary>
        public void RemoveLocalTarget() => ResetCamera();

        /// <summary>
        /// Allows to set the camera resting position externally
        /// </summary>
        public void SetTheCameraRestingPosition(Vector3 restingPosition) => cameraRestingPosition = restingPosition;
    }
}
