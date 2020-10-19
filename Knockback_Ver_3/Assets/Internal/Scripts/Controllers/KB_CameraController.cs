using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knockback.Handlers;
using EZCameraShake;
using Knockback.Utility;

namespace Knockback.Controllers
{
    public class KB_CameraController : MonoBehaviour
    {
        //todo: Commenting :: Camera controller
        //todo: Variable manipulators

        //*** Backend variables ***//

        [Header("Camera controller backend settings")]
        [Space]

        [SerializeField]
        private float cameraRestingZOffset;
        [SerializeField]
        private float cameraFOV = 5;
        [SerializeField]
        private Vector2 cameraDampingFactor;
        [SerializeField]
        private Vector2 offsetScaler;
        [SerializeField]
        private Vector2 offsetDampingFactor;
        [SerializeField]
        private Vector2 minimumBounds;
        [SerializeField]
        private Vector2 maximumBounds;

        [Header("Camera shake paramaters")]
        [Space]

        [SerializeField]
        private float roughness;
        [SerializeField]
        private float fadeInTime;
        [SerializeField]
        private float fadeOutTime;


        //** Cached variables
        private Vector3 aimOffset;
        private Vector3 targetPosition;
        private Vector3 smoothAimOffset;
        private bool canUse = false;
        private Camera mainCamera = null;
        private GameObject localTarget = null;


        protected void Awake()
        {
            transform.position += new Vector3(0, 0, cameraRestingZOffset);
            mainCamera = GetComponentInChildren<Camera>();
            mainCamera.orthographicSize = cameraFOV;

            if (TryGettingPlayerHandler())
                KB_PlayerHandler.instance.localCameraController = this;
            else
                KB_ReferenceHandler.Add(this);
        }

        private void FixedUpdate()
        {
            if (canUse)
            {
                float displacement = Vector2.Distance(transform.position, localTarget.transform.position);
                float lerpControlVariable = displacement * cameraDampingFactor.magnitude * Time.deltaTime;

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
                                offsetDampingFactor.x * Time.deltaTime
                            ),
                        Mathf.Lerp
                            (
                                smoothAimOffset.y,
                                aimOffset.y,
                                offsetDampingFactor.y * Time.deltaTime
                            ),
                        transform.position.z
                    );
                transform.position = new Vector3
                    (
                        targetPosition.x + (smoothAimOffset.x * offsetScaler.x / 10),
                        targetPosition.y + (smoothAimOffset.y * offsetScaler.y / 10),
                        transform.position.z
                    );

                //Clamping and assigning the transform
                transform.position = new Vector3
                    (
                        Mathf.Clamp(transform.position.x, minimumBounds.x, maximumBounds.x),
                        Mathf.Clamp(transform.position.y, minimumBounds.y, maximumBounds.y),
                        transform.position.z
                    );
            }
        }

        private void Update()
        {
            if (localTarget != null)
                return;

            if (TryGettingPlayerHandler())
            {
                if (KB_PlayerHandler.instance.localPlayer)
                {
                    SetLocalTarget(KB_PlayerHandler.instance.localPlayer.gameObject);
                    canUse = true;
                }
            }
        }

        private bool TryGettingPlayerHandler()
        {
            if (KB_PlayerHandler.instance == null)
                return false;
            else
                return true;
        }

        public Camera GetCamera() { return mainCamera; }

        public void AddAimOffset(Vector2 offset) { aimOffset = new Vector3(offset.x, offset.y, transform.position.z); }

        public void SetLocalTarget(GameObject localTarget) { this.localTarget = localTarget; }

        public void RemoveLocalTarget()
        {
            localTarget = null;
            canUse = false;
        }

        public void ShakeCameraWithMagnitude(float magnitude)
        {
            CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        }
    }
}
