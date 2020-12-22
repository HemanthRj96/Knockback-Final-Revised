using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knockback.Handlers;
using EZCameraShake;
using Knockback.Scriptables;

namespace Knockback.Controllers
{
    public class KB_CameraController : MonoBehaviour
    {

        #region --Attributes--


        //*** Backend variables ***//

        [Header("Camera controller backend settings")]
        [Space]

        [SerializeField]
        KB_CameraData cameraData;

        //** Cached variables
        private Vector3 aimOffset;
        private Vector3 targetPosition;
        private Vector3 smoothAimOffset;
        private bool canUse = false;
        private Camera mainCamera = null;
        public GameObject localTarget = null;


        #endregion --Attributes--

        #region --Private functions--


        private void Awake()=> CameraBootstrap();

        private void FixedUpdate()
        {
            if (localTarget == null)
                TryGettingLocalTarget();

            if (canUse)
            {
                float displacement = Vector2.Distance(transform.position, localTarget.transform.position);
                float lerpControlVariable = displacement * cameraData.cameraDampingFactor.magnitude * Time.deltaTime;

                // Tracking the target gameobject
                targetPosition = new Vector3
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

                //Adding offset and offset damping
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

                //Clamping and assigning the transform
                transform.position = new Vector3
                    (
                        Mathf.Clamp(transform.position.x, cameraData.minimumBounds.x, cameraData.maximumBounds.x),
                        Mathf.Clamp(transform.position.y, cameraData.minimumBounds.y, cameraData.maximumBounds.y),
                        transform.position.z
                    );
            }
        }

        private void CameraBootstrap()
        {
            if (cameraData == null)
                return;
            transform.position += new Vector3(0, 0, cameraData.cameraRestingZOffset);
            mainCamera = GetComponentInChildren<Camera>();
            mainCamera.orthographicSize = cameraData.cameraFOV;
            KB_ReferenceHandler.Add(this, "MainCameraController");
            canUse = true;
        }

        private void TryGettingLocalTarget() => KB_ReferenceHandler.GetReference("PlayerController", out localTarget);


#endregion --Private functions--

        #region --Public functions--


        public Camera GetCamera() => mainCamera;

        public void AddAimOffset(Vector2 offset) => aimOffset = new Vector3(offset.x, offset.y, transform.position.z);

        public void SetLocalTarget(GameObject localTarget) => this.localTarget = localTarget;

        public void RemoveLocalTarget()
        {
            localTarget = null;
            canUse = false;
        }

        public void ShakeCameraWithMagnitude(float magnitude)
        {
            CameraShaker.Instance.ShakeOnce(magnitude, cameraData.roughness, cameraData.fadeInTime, cameraData.fadeOutTime);
        }


        #endregion --Public functions--
    }
}
