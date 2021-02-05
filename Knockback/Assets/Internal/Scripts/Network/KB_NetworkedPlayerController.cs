using Mirror;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Knockback.Networked
{
    //todo: Implementation and refactoring the logic completely - KB_NetworkedPlayerController

    /// <summary>
    /// This class is a test class
    /// </summary>
    public class KB_NetworkedPlayerController : NetworkBehaviour
    {
        ////*** Settings Class ***//

        //public class ControllerSettings
        //{
        //    // Modify values here if necessary

        //    public readonly KB_NetworkedPlayerController controller = null;

        //    public readonly string movementXInputString;
        //    public readonly string movementYInputString;
        //    public readonly string jumpingInputString;
        //    public readonly string dashingInputString;
        //    public readonly string fireInputString;

        //    public readonly float moveSpeed = 8f;
        //    public readonly float jumpForce = 8f;
        //    public readonly float airControl = 0.65f;
        //    public readonly LayerMask groundCheckerLayerMask = 1 << 10;
        //    public readonly float joystickDeadzone = 0.8f;
        //    public readonly float dashingCooldown = 0.1f;
        //    public readonly float dashingSpeed = 60;
        //    public readonly float dashingDistance = 8f;

        //    public ControllerSettings() { }

        //    public ControllerSettings(KB_NetworkedPlayerController controller) { this.controller = controller; }

        //    public ControllerSettings(int[] variableModifierIndex, dynamic[] values)
        //    {
        //        if (variableModifierIndex.Length != values.Length)
        //            return;

        //        for (int index = 0; index < variableModifierIndex.Length; index++)
        //        {
        //            switch (variableModifierIndex[index])
        //            {
        //                case 0:
        //                    moveSpeed = values[index];
        //                    break;
        //                case 1:
        //                    jumpForce = values[index];
        //                    break;
        //                case 2:
        //                    airControl = values[index];
        //                    break;
        //                case 3:
        //                    groundCheckerLayerMask = values[index];
        //                    break;
        //                case 4:
        //                    joystickDeadzone = values[index];
        //                    break;
        //                case 5:
        //                    dashingCooldown = values[index];
        //                    break;
        //                case 6:
        //                    dashingSpeed = values[index];
        //                    break;
        //                case 7:
        //                    dashingDistance = values[index];
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //    public ControllerSettings(string[] inputStrings)
        //    {
        //        for (int index = 0; index < inputStrings.Length; index++)
        //        {
        //            switch (index)
        //            {
        //                case 0:
        //                    movementXInputString = inputStrings[index];
        //                    break;
        //                case 1:
        //                    movementYInputString = inputStrings[index];
        //                    break;
        //                case 2:
        //                    jumpingInputString = inputStrings[index];
        //                    break;
        //                case 3:
        //                    dashingInputString = inputStrings[index];
        //                    break;
        //                case 4:
        //                    fireInputString = inputStrings[index];
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }

        //}


        ////*** Inputs ***//

        ////*** Mobile inputs ***//

        //private float _mobileXInput => CrossPlatformInputManager.GetAxisRaw(settings.movementXInputString);
        //private float _mobileYInput => CrossPlatformInputManager.GetAxisRaw(settings.movementYInputString);
        //private float _mobileJumpInput => CrossPlatformInputManager.GetAxisRaw(settings.jumpingInputString);
        //private float _mobileDashInput => CrossPlatformInputManager.GetAxisRaw(settings.dashingInputString);
        //private float _mobileFireInput => CrossPlatformInputManager.GetAxisRaw(settings.fireInputString);

        ////*** PC inputs (Non release mode) ***//

        //private float _xInput => Input.GetAxisRaw(settings.movementXInputString);
        //private float _yInput => Input.GetAxisRaw(settings.movementYInputString);
        //private float _jumpInput => Input.GetAxisRaw(settings.jumpingInputString);
        //private float _dashInput => Input.GetAxisRaw(settings.dashingInputString);
        //private float _fireInput => Input.GetAxisRaw(settings.fireInputString);
        //private bool rightMouseButton => Input.GetKeyDown(KeyCode.Mouse1);


        //[Header("Backend settings")]
        //[Space]

        //public string xInputString;
        //public string yInputString;
        //public string jumpInputString;
        //public string dashInputString;
        //public string fireInputString;




        //public ControllerSettings settings = null;
        //private Rigidbody2D cachedRigidbody = null;
        //private SpriteRenderer cachedSpriteRenderer = null;

        //private bool canUseMouseAndKeyboard = true;
        //private bool canMove = true;
        //private bool canUse = true;
        //private bool canFly = false;



        //private void Awake()
        //{
        //    cachedRigidbody = GetComponent<Rigidbody2D>();
        //    cachedSpriteRenderer = GetComponent<SpriteRenderer>();
        //    settings = new ControllerSettings(this);
        //    settings = new ControllerSettings(new string[] { xInputString, yInputString, jumpInputString, dashInputString, fireInputString });
        //}


        //[Client]
        //private void Start()
        //{
        //    StartCoroutine(IsGrounded());
        //}


        //[Client]
        //private void Update()
        //{
        //    if (!canUse || !canMove || !hasAuthority)
        //        return;

        //    if (canUseMouseAndKeyboard)
        //    {

        //        CmdJump(_jumpInput);
        //        CmdDash(_dashInput);
        //    }
        //    else
        //    {
        //        CmdJump(_mobileJumpInput);
        //        CmdDash(_mobileDashInput);
        //    }
        //}


        //[Client]

        //private void FixedUpdate()
        //{
        //    if (!canUse || !canMove || !hasAuthority)
        //        return;

        //    if (canUseMouseAndKeyboard)
        //        CmdMove(new Vector2(_xInput, _yInput));
        //    else
        //        CmdMove(new Vector2(_mobileXInput, _mobileYInput));
        //}


        ////*** Command and rpc call for moving ***//


        //private bool rightOrLeft = false;

        ///// <summary>
        ///// Server move command
        ///// </summary>
        //[Command]
        //public void CmdMove(Vector2 axisValue)
        //{
        //    if (isDashing)
        //        return;
        //    if (Mathf.Abs(axisValue.x) < settings.joystickDeadzone && Mathf.Abs(axisValue.y) < settings.joystickDeadzone)
        //        return;
        //    RpcMove(axisValue);
        //}

        ///// <summary>
        ///// Server rpc move call
        ///// </summary>
        //[ClientRpc]
        //public void RpcMove(Vector2 axisValue)
        //{
        //    float airControl = canFly ? 1 : settings.airControl;
        //    rightOrLeft = axisValue.x > 0 ? true : (axisValue.x < 0 ? false : rightOrLeft);
        //    axisValue.y = canFly ? axisValue.y : 0;
        //    transform.position += ((new Vector3(axisValue.x * airControl, axisValue.y * airControl, 0)) * settings.moveSpeed * Time.deltaTime);
        //}


        ////*** Command and rpc call for jumping ***//


        ///// <summary>
        ///// Server jump command
        ///// </summary>
        //[Command]
        //public void CmdJump(float jumpValue)
        //{
        //    if (!isGrounded || isDashing)
        //        return;
        //    RpcJump(jumpValue);
        //}

        ///// <summary>
        ///// Server rpc jump call
        ///// </summary>
        //[ClientRpc]
        //private void RpcJump(float value) => cachedRigidbody.velocity = new Vector2(0, settings.jumpForce * value);


        ////*** Command and rpc call for dashing ***//


        //public bool canDash = true;
        //public bool isDashing = false;

        ///// <summary>
        ///// Server dash command
        ///// </summary>
        //[Command]
        //public void CmdDash(float dashValue)
        //{
        //    if (!canDash || !(dashValue > 0))
        //        return;

        //    canDash = false;
        //    RpcDash();
        //}

        ///// <summary>
        ///// Server  rpc dash call
        ///// </summary>
        //[ClientRpc]
        //private void RpcDash()
        //{
        //    StartCoroutine(StartDash());
        //}

        ///// <summary>
        ///// Coroutine for dashing
        ///// </summary>
        //private IEnumerator StartDash()
        //{
        //    isDashing = true;

        //    int direction = rightOrLeft ? 1 : -1;
        //    Vector2 initialPos = transform.position;
        //    Vector2 newPos;
        //    float elapsedTime = 0;

        //    float totalDashDistance;
        //    const float errorMargin = 0.05f;
        //    LayerMask playerIgnoreMask = 1 << 8;

        //    RaycastHit2D hit = Physics2D.Raycast(initialPos, new Vector2(direction, 0), settings.dashingDistance, ~playerIgnoreMask);
        //    if (hit.collider)
        //    {
        //        Vector2 offset = new Vector2(direction * cachedSpriteRenderer.bounds.extents.x, 0);
        //        totalDashDistance = Vector2.Distance(transform.position, hit.point - offset);
        //        Debug.Log(totalDashDistance);
        //    }
        //    else
        //        totalDashDistance = settings.dashingDistance;

        //    while (true)
        //    {
        //        float currentDistance;
        //        Vector2 offset;
        //        newPos = new Vector2(
        //                                transform.position.x + (settings.dashingSpeed * Time.deltaTime * direction),
        //                                transform.position.y
        //                            );
        //        currentDistance = Vector2.Distance(newPos, initialPos);

        //        if (currentDistance > totalDashDistance)
        //        {
        //            currentDistance -= totalDashDistance;
        //            offset = new Vector2(currentDistance + errorMargin, 0);
        //            transform.position = newPos - (offset * direction);
        //            break;
        //        }

        //        transform.position = newPos;
        //        yield return null;
        //    }

        //    isDashing = false;

        //    yield return new WaitUntil(() =>
        //    {
        //        elapsedTime += Time.deltaTime;
        //        return elapsedTime > settings.dashingCooldown;
        //    });

        //    canDash = true;
        //}


        ////*** Ground checker ***//


        //public bool isGrounded = false;

        ///// <summary>
        ///// Ground checker
        ///// </summary>
        //private IEnumerator IsGrounded()
        //{
        //    Vector2 boxSize = new Vector2(cachedSpriteRenderer.bounds.size.x - 0.5f, 0.01f);
        //    Vector3 offset = new Vector3(0f, cachedSpriteRenderer.bounds.extents.y + boxSize.y, 0f);
        //    while (gameObject.activeInHierarchy)
        //    {
        //        isGrounded = Physics2D.BoxCast(transform.position - offset, boxSize, 0, Vector2.down, boxSize.y, settings.groundCheckerLayerMask).collider != null;
        //        yield return null;
        //    }
        //    yield return null;
        //}
    }
}