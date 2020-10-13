using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Mirror;
using System.Collections;
using Knockback.Handlers;
using Knockback.Utils;

namespace Knockback.Controllers
{
    public class KB_PlayerController : MonoBehaviour
    {
        // todo: Convert all of them to readonly
        // todo: Implement player trail; Implement it another script
        // todo: Implement the player health and armor implementation as another internal class
        // todo: Network implementation
        // todo: Commenting :: PlayerController

        //*** Backend variables ***//

        [Header("Player backend settings")]

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
        private float moveSpeed = 2.4f;
        [SerializeField]
        private float jumpForce = 24f;
        [SerializeField]
        private float airControl = 0.65f;
        [SerializeField]
        private LayerMask groundCheckerLayerMask;
        [Range(0, 1)]
        [SerializeField]
        private float joystickDeadzone = 0.8f;
        [SerializeField]
        private bool useMouseAndKeyboard;
        [SerializeField]
        private Rigidbody2D cachedRigidbody = null;
        [SerializeField]
        private SpriteRenderer cachedSpriteRenderer = null;

        private readonly float dashingCooldown = 0.87f;
        private readonly float dashingSpeed = 30;
        private readonly float dashingDistance = 4.5f;

        private bool canUse = false;

        //*** Reference variables ***//

        private float xInput =>
        useMouseAndKeyboard ? Input.GetAxisRaw(movementXInputString) : CrossPlatformInputManager.GetAxisRaw(movementXInputString);
        private float yInput =>
        useMouseAndKeyboard ? Input.GetAxisRaw(movementYInputString) : CrossPlatformInputManager.GetAxisRaw(movementYInputString);
        private bool jumpInput =>
        useMouseAndKeyboard ? Input.GetButtonDown(jumpingInputString) : CrossPlatformInputManager.GetButtonDown(jumpingInputString);
        private bool dashInput =>
        useMouseAndKeyboard ? Input.GetButtonDown(dashingInputString) : CrossPlatformInputManager.GetButtonDown(dashingInputString);
        private bool fireInput =>
        useMouseAndKeyboard ? Input.GetButtonDown(fireInputString) : CrossPlatformInputManager.GetButtonDown(fireInputString);
        public Transform weaponSlot => transform.GetChild(1);


        //*** Variables synchronized over network ***//

        //[HideInInspector]
        //[SyncVar]
        public bool canMove = false;
        [HideInInspector]
        //[SyncVar]
        public bool canFly = false;
        [HideInInspector]
        //[SyncVar]
        public float health;
        //[HideInInspector]
        //[SyncVar]
        public bool isReady = false;


        //*** Cached variables ***//

        private KB_PlayerInventoryHandler playerInventory = null;
        private PlayerLocomotion playerLocomotion = null;
        private PlayerLookRotation playerLookRotation = null;
        private PlayerWeaponHandler playerWeaponHandler = null;

        //*** Default functions ***//

        private void Awake()
        {
            playerInventory = GetComponent<KB_PlayerInventoryHandler>();
            playerLocomotion = new PlayerLocomotion(this);
            playerLookRotation = new PlayerLookRotation(this);
            playerWeaponHandler = new PlayerWeaponHandler(this);

            if (cachedRigidbody != null & cachedSpriteRenderer != null & playerInventory != null)
                canUse = true;
            else
                new KBLog($"Missing component reference: SpriteRenderer/Rigidbody/PlayerInventory : {cachedSpriteRenderer}{cachedRigidbody}{playerInventory}");
        }

        private void Update()
        {
            if (!canUse)
                return;
            if (!isReady)
                return;
            CmdUpdateButtonStates();
            CmdUpdateRotation();
        }

        private void FixedUpdate()
        {
            if (!canUse)
                return;
            if (!isReady)
                return;
            CmdUpdateAxisStates();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        //*** Command functions called from the client to execute methods on server ***//

        /// <summary>
        /// Server command to update axis values
        /// </summary>
        //[Command]
        private void CmdUpdateAxisStates()
        {
            if (!canMove/* || !hasAuthority*/)
                return;

            playerLocomotion.RpcMove(new Vector2(xInput, yInput));
        }

        /// <summary>
        /// Server command to update button states
        /// </summary>
        //[Command]
        private void CmdUpdateButtonStates()
        {
            if (!canMove/* || !hasAuthority*/)
                return;

            playerLocomotion.RpcJump(jumpInput);
            playerLocomotion.RpcDash(dashInput);
            playerWeaponHandler.RpcUseWeapon(fireInput);
        }

        /// <summary>
        /// Server command to update rotation
        /// </summary>
        //[Command]
        private void CmdUpdateRotation()
        {
            if (!canMove/* || !hasAuthority*/)
                return;

            weaponSlot.rotation = playerLookRotation.GetCalculatedRotation();
        }

        //*** Internal class ***//

        /// <summary>
        /// This class primarly handles the player locomotion
        /// </summary>
        internal class PlayerLocomotion
        {
            public PlayerLocomotion(KB_PlayerController controller)
            {
                this.controller = controller;
            }

            private KB_PlayerController controller = null;

            private bool isGrounded = true;
            private bool rightOrLeft = true;
            private bool canDash = true;
            private bool isDashing = false;
            private bool isGroundCheckerRunning = false;

            //[ClientRpc]
            public void RpcMove(Vector2 axisValue)
            {
                if (!isGroundCheckerRunning)
                    controller.StartCoroutine(IsGrounded());

                if (isDashing)
                    return;
                if (Mathf.Abs(axisValue.x) < controller.joystickDeadzone && Mathf.Abs(axisValue.y) < controller.joystickDeadzone)
                    return;

                rightOrLeft = axisValue.x > 0 ? true : (axisValue.x < 0 ? false : rightOrLeft);
                float airControl = controller.canFly ? 1 : controller.airControl;
                axisValue.y = controller.canFly ? axisValue.y : 0;
                controller.transform.position += ((new Vector3(axisValue.x * airControl, axisValue.y * airControl, 0)) * controller.moveSpeed * Time.deltaTime);
            }

            //[ClientRpc]
            public void RpcJump(bool value)
            {
                if (!isGroundCheckerRunning)
                    controller.StartCoroutine(IsGrounded());
                if (!isGrounded || isDashing || !value)
                    return;

                controller.cachedRigidbody.velocity = new Vector2(0, controller.jumpForce) * 5;
            }

            //[ClientRpc]
            public void RpcDash(bool value)
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
                    isGrounded = Physics2D.BoxCast(controller.transform.position - offset, boxSize, 0, Vector2.down, boxSize.y, controller.groundCheckerLayerMask).collider != null;
                    yield return null;
                }
                isGroundCheckerRunning = false;
                yield return null;
            }

            private IEnumerator StartDash()
            {
                Vector3 initialPos = controller.transform.position;
                int direction = rightOrLeft ? 1 : -1;
                float dashingTimer = 0;
                isDashing = true;
                while (Vector3.Distance(controller.transform.position, initialPos) < controller.dashingDistance)
                {
                    controller.transform.position += ((new Vector3(direction, 0, 0)) * controller.dashingSpeed * Time.deltaTime);
                    yield return new WaitForFixedUpdate();
                }
                isDashing = false;
                yield return new WaitUntil(() =>
                    {
                        dashingTimer += Time.deltaTime;
                        return dashingTimer > controller.dashingCooldown;
                    });
                canDash = true;
            }
        }

        internal class PlayerLookRotation
        {
            public PlayerLookRotation(KB_PlayerController controller)
            {
                this.controller = controller;
            }

            KB_PlayerController controller = null;

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
            //[ClientRpc]
            public void RpcUseWeapon(bool value)
            {
                if (!value)
                    return;
                if (controller.weaponSlot.childCount == 0)
                    return;
                if (activeIndex == -1)
                {
                    new KBLog("Slots cannot be used");
                    return;
                }

                if (cachedItem == null)
                {
                    for (int index = 0; index < controller.weaponSlot.childCount; index++)
                    {
                        if (controller.weaponSlot.GetChild(index).gameObject.activeInHierarchy)
                        {
                            cachedItem = controller.weaponSlot.GetChild(index).gameObject;
                            Use(cachedItem.GetComponent<IUsableEntity>());
                            break;
                        }
                    }
                    return;
                }
                else if (!cachedItem.activeInHierarchy)
                {
                    cachedItem = null;
                    for (int index = 0; index < controller.weaponSlot.childCount; index++)
                    {
                        if (controller.weaponSlot.GetChild(index).gameObject.activeInHierarchy)
                        {
                            cachedItem = controller.weaponSlot.GetChild(index).gameObject;
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