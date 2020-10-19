using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using Knockback.Handlers;
using Knockback.Utility;
using Unity.Mathematics;

namespace Knockback.Controllers
{
    public class KB_PlayerController : MonoBehaviour
    {
        // todo: Convert all of them to readonly
        // todo: Implement player trail; Implement it another script
        // todo: Commenting :: PlayerController        

        //*** Settings Class ***//

        public class ControllerSettings
        {
            // Modify values here if necessary

            public readonly KB_PlayerController controller = null;

            public readonly string movementXInputString;
            public readonly string movementYInputString;
            public readonly string jumpingInputString;
            public readonly string dashingInputString;
            public readonly string fireInputString;

            public readonly float moveSpeed = 10f;
            public readonly float jumpForce = 5.5f;
            public readonly float airControl = 0.65f;
            public readonly LayerMask groundCheckerLayerMask = 1 << 10;
            public readonly float dashingCooldown = 0.85f;
            public readonly float dashingSpeed = 60;
            public readonly float dashingDistance = 4.5f;

            public readonly float joystickDeadzone = 0.8f;

            public ControllerSettings() { }

            public ControllerSettings(KB_PlayerController controller) { this.controller = controller; }

            public ControllerSettings(int[] variableModifierIndex, dynamic[] values)
            {
                if (variableModifierIndex.Length != values.Length)
                    return;

                for (int index = 0; index < variableModifierIndex.Length; index++)
                {
                    switch (variableModifierIndex[index])
                    {
                        case 0:
                            moveSpeed = values[index];
                            break;
                        case 1:
                            jumpForce = values[index];
                            break;
                        case 2:
                            airControl = values[index];
                            break;
                        case 3:
                            groundCheckerLayerMask = values[index];
                            break;
                        case 4:
                            joystickDeadzone = values[index];
                            break;
                        case 5:
                            dashingCooldown = values[index];
                            break;
                        case 6:
                            dashingSpeed = values[index];
                            break;
                        case 7:
                            dashingDistance = values[index];
                            break;
                        default:
                            break;
                    }
                }
            }

            public ControllerSettings(string[] inputStrings)
            {
                for (int index = 0; index < inputStrings.Length; index++)
                {
                    switch (index)
                    {
                        case 0:
                            movementXInputString = inputStrings[index];
                            break;
                        case 1:
                            movementYInputString = inputStrings[index];
                            break;
                        case 2:
                            jumpingInputString = inputStrings[index];
                            break;
                        case 3:
                            dashingInputString = inputStrings[index];
                            break;
                        case 4:
                            fireInputString = inputStrings[index];
                            break;
                        default:
                            break;
                    }
                }
            }

        }


        //*** Backend variables ***//

        [SerializeField]
        private string movementXInputString;
        [SerializeField]
        private string movementYInputString;
        [SerializeField]
        private string jumpingInputString;
        [SerializeField]
        private string dashingInputString;
        [SerializeField]
        private string fireInputString;

        [SerializeField]
        private bool canUse = false;
        [SerializeField]
        private bool useMouseAndKeyboard = true;

        public GameObject impactPoint = null;
        public float adjustments;

        //*** Inputs ***//

        //*** Mobile inputs ***//

        private float _mobielXInput => CrossPlatformInputManager.GetAxisRaw(settings.movementXInputString);
        private float _mobileYInput => CrossPlatformInputManager.GetAxisRaw(settings.movementYInputString);
        private bool _mobileJumpInput => CrossPlatformInputManager.GetButtonDown(settings.jumpingInputString);
        private bool _mobileDashInput => CrossPlatformInputManager.GetButtonDown(settings.dashingInputString);
        private bool _mobileFireInput => CrossPlatformInputManager.GetButtonDown(settings.fireInputString);

        //*** PC inputs (Non release mode) ***//

        private float xInput => Input.GetAxisRaw(settings.movementXInputString);
        private float yInput => Input.GetAxisRaw(settings.movementYInputString);
        private bool jumpInput => Input.GetButtonDown(settings.jumpingInputString);
        private bool dashInput => Input.GetButtonDown(settings.dashingInputString);
        private bool fireInput => Input.GetButtonDown(settings.fireInputString);
        private bool rightMouseButton => Input.GetKeyDown(KeyCode.Mouse1);

        //*** Reference variables ***//


        //*** Cached variables ***//

        private ControllerSettings settings = null;
        private KB_PlayerInventoryHandler playerInventory = null;
        private PlayerLocomotion playerLocomotion = null;
        private PlayerLookRotation playerLookRotation = null;
        private PlayerWeaponHandler playerWeaponHandler = null;
        private SpriteRenderer cachedSpriteRenderer = null;
        private Rigidbody2D cachedRigidbody = null;
        private Transform cachedWeaponSlot = null;

        private bool canMove = true;
        private bool canFly = false;
        private bool isReady = true;


        //*** Default functions ***//

        private void Awake()
        {
            settings = new ControllerSettings(this);
            settings = new ControllerSettings
                (new string[] { movementXInputString, movementYInputString, jumpingInputString, dashingInputString, fireInputString });

            playerInventory = GetComponent<KB_PlayerInventoryHandler>();
            playerLocomotion = new PlayerLocomotion(this);
            playerLookRotation = new PlayerLookRotation(this);
            playerWeaponHandler = new PlayerWeaponHandler(this);

            cachedWeaponSlot = transform.GetChild("_WeaponSlot");
            cachedSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            cachedRigidbody = GetComponent<Rigidbody2D>();

            if (cachedRigidbody != null & cachedSpriteRenderer != null & playerInventory != null)
                canUse = true;
            else
                new KBLog($"Missing component reference: SpriteRenderer/Rigidbody/PlayerInventory : {cachedSpriteRenderer}{cachedRigidbody}{playerInventory}");
        }

        private void Update()
        {
            if (!canUse || !isReady || !canMove)
                return;
            playerLocomotion.Jump(jumpInput);
            playerLocomotion.Dash(dashInput);
            playerWeaponHandler.UseWeapon(fireInput);
            UpdateRotation();
        }

        private void FixedUpdate()
        {
            if (!canUse || !isReady || !canMove)
                return;
            playerLocomotion.Move(new Vector2(xInput, yInput));
        }

        private void OnDestroy() => StopAllCoroutines();

        private void UpdateRotation() => cachedWeaponSlot.rotation = playerLookRotation.GetCalculatedRotation();

        public void ModifySettings(int[] index, dynamic[] values) => settings = new ControllerSettings(index, values);

        public ControllerSettings GetSettings() => settings;

        //*** Internal class ***//

        /// <summary>
        /// This class primarly handles the player locomotion
        /// </summary>
        internal class PlayerLocomotion
        {
            public PlayerLocomotion(KB_PlayerController controller)
            {
                this.controller = controller;
                settings = controller.settings;
            }

            private KB_PlayerController controller = null;
            private ControllerSettings settings = null;

            private bool isGrounded = true;
            private bool rightOrLeft = true;
            private bool canDash = true;
            private bool isDashing = false;
            private bool isGroundCheckerRunning = false;
            public bool stopDash = false;

            public void Move(Vector2 axisValue)
            {
                if (!isGroundCheckerRunning)
                    controller.StartCoroutine(IsGrounded());

                if (isDashing)
                    return;
                if (Mathf.Abs(axisValue.x) < settings.joystickDeadzone && Mathf.Abs(axisValue.y) < settings.joystickDeadzone)
                    return;

                rightOrLeft = axisValue.x > 0 ? true : (axisValue.x < 0 ? false : rightOrLeft);
                float airControl = controller.canFly ? 1 : settings.airControl;
                axisValue.y = controller.canFly ? axisValue.y : 0;
                controller.transform.position += ((new Vector3(axisValue.x * airControl, axisValue.y * airControl, 0)) * settings.moveSpeed * Time.deltaTime);
            }

            public void Jump(bool value)
            {
                if (!isGroundCheckerRunning)
                    controller.StartCoroutine(IsGrounded());
                if (!isGrounded || isDashing || !value)
                    return;

                controller.cachedRigidbody.velocity = new Vector2(0, settings.jumpForce) * 5;
            }

            public void Dash(bool value)
            {
                if (!isGroundCheckerRunning)
                    controller.StartCoroutine(IsGrounded());
                if (!canDash || !value)
                    return;

                canDash = false;
                controller.StartCoroutine(StartDash());
            }

            private IEnumerator IsGrounded()
            {
                isGroundCheckerRunning = true;
                Vector2 boxSize = new Vector2(controller.cachedSpriteRenderer.bounds.size.x - 0.2f, 0.01f);
                Vector3 offset = new Vector3(0f, controller.cachedSpriteRenderer.bounds.extents.y + boxSize.y, 0f);
                while (controller.gameObject.activeInHierarchy)
                {
                    isGrounded = Physics2D.BoxCast(controller.transform.position - offset, boxSize, 0, Vector2.down, boxSize.y, settings.groundCheckerLayerMask).collider != null;
                    yield return null;
                }
                isGroundCheckerRunning = false;
                yield return null;
            }

            private IEnumerator StartDash()
            {
                isDashing = true;

                int direction = rightOrLeft ? 1 : -1;
                Vector2 initialPos = controller.transform.position;
                Vector2 newPos;
                float elapsedTime = 0;
                float totalDashDistance = 0;
                const float errorMargin = 0.01f;
                LayerMask playerIgnoreMask = 1 << 8;

                RaycastHit2D hit = Physics2D.Raycast(initialPos, new Vector2(direction, 0), settings.dashingDistance, ~playerIgnoreMask);
                if (hit.collider)
                {
                    Vector2 offset = new Vector2(direction * controller.cachedSpriteRenderer.bounds.extents.x, 0);
                    totalDashDistance = Vector2.Distance(controller.transform.position, hit.point - offset);
                }
                else
                    totalDashDistance = settings.dashingDistance;

                while (true)
                {
                    float currentDistance = 0;
                    Vector2 offset;
                    newPos = new Vector2(
                                            controller.transform.position.x + (settings.dashingSpeed * Time.deltaTime * direction),
                                            controller.transform.position.y
                                        );
                    currentDistance = Vector2.Distance(newPos, initialPos);

                    if (currentDistance > totalDashDistance)
                    {
                        currentDistance -= totalDashDistance;
                        offset = new Vector2(currentDistance + errorMargin, 0);
                        controller.transform.position = newPos - (offset * direction);
                        break;
                    }

                    controller.transform.position = newPos;
                    yield return null;
                }

                isDashing = false;

                yield return new WaitUntil(() =>
                {
                    elapsedTime += Time.deltaTime;
                    return elapsedTime > settings.dashingCooldown;
                });

                canDash = true;
            }
        }

        /// <summary>
        /// This class handles the rotation  of the weapon handle
        /// </summary>
        internal class PlayerLookRotation
        {
            public PlayerLookRotation(KB_PlayerController controller)
            {
                this.controller = controller;
                settings = controller.settings;
            }

            KB_PlayerController controller = null;
            ControllerSettings settings = null;

            private KB_CameraController localCameraController { get; set; } = null;
            private Camera localMainCamera { get; set; } = null;

            private float rotationDegress = 0;
            private bool useAlternateInput = false;

            public Quaternion GetCalculatedRotation()
            {
                Vector2 difference = Vector2.zero;
                Quaternion targetRotation = Quaternion.identity;

                if (controller.useMouseAndKeyboard && !useAlternateInput)
                {
                    if (localCameraController == null)
                    {
                        if (!TryGettingCamera())
                        {
                            new KBLog("Unable to find camera controller using joystick as input", 0);
                            useAlternateInput = true;
                            difference = new Vector2(controller.xInput, controller.yInput);
                        }
                    }
                    else
                    {
                        difference = localMainCamera.ScreenToWorldPoint(Input.mousePosition) - controller.transform.position;
                        difference.Normalize();
                    }
                }
                else
                {
                    difference = new Vector2(controller.xInput, controller.yInput);
                }

                if (difference != Vector2.zero)
                    rotationDegress = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                targetRotation = Quaternion.Euler(0, 0, rotationDegress);

                if (controller.useMouseAndKeyboard && controller.rightMouseButton)
                    localCameraController?.AddAimOffset(difference);
                else
                    localCameraController?.AddAimOffset(difference);

                if (rotationDegress > 90 || rotationDegress < -90)
                {
                    controller.cachedSpriteRenderer.flipX = true;
                    targetRotation = Quaternion.Euler(180, 0, -rotationDegress);
                }
                else
                    controller.cachedSpriteRenderer.flipX = false;

                return targetRotation;
            }

            private bool TryGettingCamera()
            {
                try
                {
                    if (KB_PlayerHandler.instance.localCameraController == null)
                    {
                        KB_CameraController cameraController;
                        if (KB_ReferenceHandler.GetReference(out cameraController))
                        {
                            localCameraController = cameraController;
                            localMainCamera = localCameraController.GetCamera();
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        localCameraController = KB_PlayerHandler.instance.localCameraController;
                        localMainCamera = KB_PlayerHandler.instance.localCameraController.GetCamera();
                        return true;
                    }
                }
                catch (System.Exception)
                {
                    KB_CameraController cameraController;
                    if (KB_ReferenceHandler.GetReference(out cameraController))
                    {
                        localCameraController = cameraController;
                        localMainCamera = localCameraController.GetCamera();
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// This class handles the all weapons in the item slot
        /// </summary>
        internal class PlayerWeaponHandler
        {
            public PlayerWeaponHandler(KB_PlayerController controller)
            {
                this.controller = controller;
            }

            private KB_PlayerController controller;
            private int activeIndex => controller.playerInventory.GetActiveSlotIndex();
            private GameObject cachedItem { get; set; }


            /// <summary>
            /// Rpc function call to invoke the useTtem function in the IteractableItem
            /// </summary>
            public void UseWeapon(bool value)
            {
                if (!value)
                    return;
                if (controller.cachedWeaponSlot.childCount == 0)
                    return;
                if (activeIndex == -1)
                {
                    new KBLog("Slots cannot be used");
                    return;
                }

                if (cachedItem == null)
                {
                    for (int index = 0; index < controller.cachedWeaponSlot.childCount; index++)
                    {
                        if (controller.cachedWeaponSlot.GetChild(index).gameObject.activeInHierarchy)
                        {
                            cachedItem = controller.cachedWeaponSlot.GetChild(index).gameObject;
                            Use(cachedItem.GetComponent<IUsableEntity>());
                            break;
                        }
                    }
                    return;
                }
                else if (!cachedItem.activeInHierarchy)
                {
                    cachedItem = null;
                    for (int index = 0; index < controller.cachedWeaponSlot.childCount; index++)
                    {
                        if (controller.cachedWeaponSlot.GetChild(index).gameObject.activeInHierarchy)
                        {
                            cachedItem = controller.cachedWeaponSlot.GetChild(index).gameObject;
                            Use(cachedItem.GetComponent<IUsableEntity>());
                            break;
                        }
                    }
                    return;
                }
                else
                    Use(cachedItem.GetComponent<IUsableEntity>());
            }

            private void Use(IUsableEntity item) { item.UseItem(controller.gameObject); }
        }
    }
}
