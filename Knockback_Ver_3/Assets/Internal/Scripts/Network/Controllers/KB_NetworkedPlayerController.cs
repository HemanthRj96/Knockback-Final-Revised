using Knockback.Controllers;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class KB_NetworkedPlayerController : NetworkBehaviour
{
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

        public readonly float moveSpeed = 5f;
        public readonly float jumpForce = 8f;
        public readonly float airControl = 0.65f;
        public readonly LayerMask groundCheckerLayerMask = 1 << 8;
        public readonly float joystickDeadzone = 0.8f;
        public readonly float dashingCooldown = 0.87f;
        public readonly float dashingSpeed = 30;
        public readonly float dashingDistance = 4.5f;

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


    //*** Inputs ***//

    //*** Mobile inputs ***//

    private float _mobielXInput => CrossPlatformInputManager.GetAxisRaw(settings.movementXInputString);
    private float _mobileYInput => CrossPlatformInputManager.GetAxisRaw(settings.movementYInputString);
    private float _mobileJumpInput => CrossPlatformInputManager.GetAxisRaw(settings.jumpingInputString);
    private float _mobileDashInput => CrossPlatformInputManager.GetAxisRaw(settings.dashingInputString);
    private float _mobileFireInput => CrossPlatformInputManager.GetAxisRaw(settings.fireInputString);

    //*** PC inputs (Non release mode) ***//

    private float xInput => Input.GetAxisRaw(settings.movementXInputString);
    private float yInput => Input.GetAxisRaw(settings.movementYInputString);
    private float jumpInput => Input.GetAxisRaw(settings.jumpingInputString);
    private float dashInput => Input.GetAxisRaw(settings.dashingInputString);
    private float fireInput => Input.GetAxisRaw(settings.fireInputString);
    private bool rightMouseButton => Input.GetKeyDown(KeyCode.Mouse1);


    [Header("Backend settings")]
    [Space]

    public string xInputString;
    public string yInputString;
    public string jumpInputString;
    public string dashInputString;
    public string fireInputString;




    public ControllerSettings settings = new ControllerSettings();

    private Rigidbody2D cachedRigidbody = null;
    private SpriteRenderer cachedSpriteRenderer = null;


    private bool canMove = true;
    private bool canUse = true;
    private bool canFly = false;



    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody2D>();
        cachedSpriteRenderer = GetComponent<SpriteRenderer>();
        settings = new ControllerSettings(new string[] { xInputString, yInputString, jumpInputString, dashInputString, fireInputString });
    }


    [Client]
    private void Start()
    {
        StartCoroutine(IsGrounded());
    }


    [Client]
    private void Update()
    {
        if (!canUse || !canMove || !hasAuthority)
            return;

        CmdJump(jumpInput);
        CmdDash(dashInput);
    }


    [Client]

    private void FixedUpdate()
    {
        if (!canUse || !canMove || !hasAuthority)
            return;

        CmdMove(new Vector2(xInput, yInput));
    }


    //*** Command and rpc call for moving ***//


    private bool rightOrLeft = false;

    /// <summary>
    /// Server move command
    /// </summary>
    [Command]
    public void CmdMove(Vector2 axisValue)
    {
        if (isDashing)
            return;
        if (Mathf.Abs(axisValue.x) < settings.joystickDeadzone && Mathf.Abs(axisValue.y) < settings.joystickDeadzone)
            return;
        RpcMove(axisValue);
    }

    /// <summary>
    /// Server rpc move call
    /// </summary>
    [ClientRpc]
    public void RpcMove(Vector2 axisValue)
    {
        float airControl = canFly ? 1 : settings.airControl;
        rightOrLeft = axisValue.x > 0 ? true : (axisValue.x < 0 ? false : rightOrLeft);
        axisValue.y = canFly ? axisValue.y : 0;
        transform.position += ((new Vector3(axisValue.x * airControl, axisValue.y * airControl, 0)) * settings.moveSpeed * Time.deltaTime);
    }


    //*** Command and rpc call for jumping ***//


    /// <summary>
    /// Server jump command
    /// </summary>
    [Command]
    public void CmdJump(float jumpValue)
    {
        if (!isGrounded || isDashing)
            return;
        RpcJump(jumpValue);
    }

    /// <summary>
    /// Server rpc jump call
    /// </summary>
    [ClientRpc]
    private void RpcJump(float value) => cachedRigidbody.velocity = new Vector2(0, settings.jumpForce * value);


    //*** Command and rpc call for dashing ***//


    private bool canDash = false;
    private bool isDashing = false;

    /// <summary>
    /// Server dash command
    /// </summary>
    [Command]
    public void CmdDash(float dashValue)
    {
        if (!canDash)
            return;
    }

    /// <summary>
    /// Server  rpc dash call
    /// </summary>
    [ClientRpc]
    private void RpcDash(bool dashValue)
    {
        canDash = false;
        StartCoroutine(StartDash());
    }

    /// <summary>
    /// Coroutine for dashing
    /// </summary>
    private IEnumerator StartDash()
    {
        Vector3 initialPos = transform.position;
        int direction = rightOrLeft ? 1 : -1;
        float dashingTimer = 0;
        isDashing = true;
        while (Vector3.Distance(transform.position, initialPos) < settings.dashingDistance)
        {
            transform.position += ((new Vector3(direction, 0, 0)) * settings.dashingSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        isDashing = false;
        yield return new WaitUntil(() =>
        {
            dashingTimer += Time.deltaTime;
            return dashingTimer > settings.dashingCooldown;
        });
        canDash = true;
    }


    //*** Ground checker ***//


    private bool isGrounded = false;

    /// <summary>
    /// Ground checker
    /// </summary>
    private IEnumerator IsGrounded()
    {
        Vector2 boxSize = new Vector2(cachedSpriteRenderer.bounds.size.x - 0.2f, 0.01f);
        Vector3 offset = new Vector3(0f, cachedSpriteRenderer.bounds.extents.y + boxSize.y, 0f);
        while (gameObject.activeInHierarchy)
        {
            isGrounded = Physics2D.BoxCast(transform.position - offset, boxSize, 0, Vector2.down, boxSize.y, settings.groundCheckerLayerMask).collider != null;
            yield return null;
        }
        yield return null;
    }


}
