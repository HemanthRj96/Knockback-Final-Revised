using UnityEngine;
using Knockback.Utility;
using Knockback.Handlers;
using Knockback.Helpers;
using System.Collections;

namespace Knockback.Controllers
{
    public class KB_PlayerController : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("---Backend Settings---")]
        [SerializeField] private KB_InputSettings _inputSettings = new KB_InputSettings();
        [SerializeField] private KB_PlayerBackendSettings _playerSettings = new KB_PlayerBackendSettings();
        [SerializeField] private bool canUse = false;

        //** --PUBLIC ATTRIBUTES--

        [Header("---Cached Components---")]
        public SpriteRenderer cachedSpriteRenderer = null;
        public Rigidbody2D cachedRigidbody = null;
        public Transform cachedWeaponSlot = null;

        //** --PRIVATE ATTRIBUTES--

        private KB_InventoryHandler _inventoryHandler = new KB_InventoryHandler();
        private KB_Locomotion _locomotionHandler = new KB_Locomotion();
        private KB_PlayerSlotRotation _playerLookRotation = new KB_PlayerSlotRotation();
        private KB_PlayerItemHandler _itemHandler = new KB_PlayerItemHandler();
        private KB_PlayerKnockbackHandler _knockbackHandler = new KB_PlayerKnockbackHandler();
        private bool canMove = true;
        private bool isReady = true;
        private bool isNetworked = false;
        private const string _CAMERA_CONTROLLER_TAG = "MainCameraController";

        //** --PUBLIC REFERENCES--

        public KB_InventoryHandler inventoryHandler { get { return _inventoryHandler; } private set { _inventoryHandler = value; } }
        public KB_Locomotion locomotionHandler { get { return _locomotionHandler; } private set { _locomotionHandler = value; } }
        public KB_InputSettings inputSettings { get { return _inputSettings; } private set { _inputSettings = value; } }
        public KB_PlayerBackendSettings playerSettings { get { return _playerSettings; } private set { _playerSettings = value; } }
        public KB_PlayerSlotRotation playerSlotRotation { get { return _playerLookRotation; } private set { _playerLookRotation = value; } }
        public KB_PlayerItemHandler itemHandler { get { return _itemHandler; } private set { _itemHandler = value; } }
        public KB_PlayerKnockbackHandler knockbackHandler { get { return _knockbackHandler; } private set { _knockbackHandler = value; } }


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Externally modify the player backend settings like speed, jump height etc..
        /// </summary>
        /// <param name="settingType">Array of setting types that has to be changed</param>
        /// <param name="values">Array of new values</param>
        public void ModifySettings(PlayerBackendSettingType[] settingType, dynamic[] values) => playerSettings = new KB_PlayerBackendSettings(settingType, values);

        /// <summary>
        /// Getter for settings
        /// </summary>
        public KB_PlayerBackendSettings GetSettings() => playerSettings;

        //** --PRIVATE METHODS--

        /// <summary>
        /// Player controller boostrapper routine
        /// </summary>
        private void Awake()
        {
            if (!CheckComponentReferenceValidity())
                return;
            canUse = true;
            PlayerControllerBootstrap();
        }


        /// <summary>
        /// Set the camera target as this gameObject
        /// </summary>
        private void Start()
        {
            (KB_ReferenceHandler.GetReference(_CAMERA_CONTROLLER_TAG) as KB_CameraController)?.SetLocalTarget(gameObject);
        }

        /// <summary>
        /// Update the input and rotation
        /// </summary>
        private void Update()
        {
            if (!canUse || !isReady || !canMove)
                return;
            UpdateRoutine();
        }

        /// <summary>
        /// Update the movement of the player
        /// </summary>
        private void FixedUpdate()
        {
            if (!canUse || !isReady || !canMove)
                return;
            FixedUpdateRoutine();
        }

        /// <summary>
        /// Returns true if the references are valid
        /// </summary>
        private bool CheckComponentReferenceValidity()
        {
            if (cachedRigidbody != null & cachedSpriteRenderer != null)
                return true;
            else
            {
                new KBLog($"Missing component reference: SpriteRenderer/Rigidbody/PlayerInventory : {cachedSpriteRenderer}{cachedRigidbody}");
                return false;
            }
        }

        /// <summary>
        /// Bootstrapper method
        /// </summary>
        private void PlayerControllerBootstrap()
        {
            inventoryHandler = new KB_InventoryHandler(this);
            locomotionHandler = new KB_Locomotion(this);
            playerSettings = new KB_PlayerBackendSettings();
            playerSlotRotation = new KB_PlayerSlotRotation(this);
            itemHandler = new KB_PlayerItemHandler(this);
            knockbackHandler = new KB_PlayerKnockbackHandler(this, cachedRigidbody);
            StartCoroutine(InventorySlotLoader());
        }

        /// <summary>
        /// Tries slot load after some buffer time
        /// </summary>
        private IEnumerator InventorySlotLoader()
        {
            yield return new WaitForSeconds(1);
            inventoryHandler.TrySlotLoad();
        }

        /// <summary>
        /// This is the update routine that runs with the update call
        /// </summary>
        private void UpdateRoutine()
        {
            if (!isNetworked)
            {
                OfflineInputUpdate();
                OfflineRotationUpdate();
            }
            else
            {
                // Do things if networked
            }
        }

        /// <summary>
        /// This is the fixed update routine that runs with the fixed update
        /// </summary>
        private void FixedUpdateRoutine()
        {
            if (!isNetworked)
            {
                OfflineMovementUpdate();
            }
            else
            {
                // Do something if networked
            }
        }

        /// <summary>
        /// Offline input update
        /// </summary>
        private void OfflineInputUpdate()
        {
            locomotionHandler.Jump(inputSettings.JumpInput());
            itemHandler.UseWeapon(inputSettings.FireInput());
        }

        /// <summary>
        /// Online input update
        /// </summary>
        private void OnlineInputUpdate()
        {

        }

        /// <summary>
        /// Offline rotation update
        /// </summary>
        private void OfflineRotationUpdate()
        {
            cachedWeaponSlot.rotation = playerSlotRotation.GetCalculatedRotation();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnlineRotationUpdate()
        {

        }

        /// <summary>
        /// Offline movement update
        /// </summary>
        private void OfflineMovementUpdate()
        {
            locomotionHandler.Move(inputSettings.MovementInput().x);
            locomotionHandler.Dash(inputSettings.DashInput());
        }

        /// <summary>
        /// Online movement update
        /// </summary>
        private void OnlineMovementUpdate()
        {

        }        
    }
}
