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
        [SerializeField] private KB_InputSettings m_inputSettings = new KB_InputSettings();
        [SerializeField] private KB_PlayerBackendSettings m_playerSettings = new KB_PlayerBackendSettings();
        [SerializeField] private bool m_canUse = false;

        //** --PUBLIC ATTRIBUTES--

        [Header("---Cached Components---")]
        public SpriteRenderer m_cachedSpriteRenderer = null;
        public Rigidbody2D m_cachedRigidbody = null;
        public Transform m_cachedWeaponSlot = null;

        //** --PRIVATE ATTRIBUTES--

        private KB_InventoryHandler m_inventoryHandler = new KB_InventoryHandler();
        private KB_Locomotion m_locomotionHandler = new KB_Locomotion();
        private KB_PlayerSlotRotation m_playerLookRotation = new KB_PlayerSlotRotation();
        private KB_PlayerItemHandler m_itemHandler = new KB_PlayerItemHandler();
        private KB_PlayerKnockbackHandler m_knockbackHandler = new KB_PlayerKnockbackHandler();
        private bool m_canMove = true;
        private bool m_isReady = true;
        private bool m_isNetworked = false;
        private const string _CAMERA_CONTROLLER_TAG = "MainCameraController";

        //** --PUBLIC REFERENCES--

        public KB_InventoryHandler inventoryHandler { get { return m_inventoryHandler; } private set { m_inventoryHandler = value; } }
        public KB_Locomotion locomotionHandler { get { return m_locomotionHandler; } private set { m_locomotionHandler = value; } }
        public KB_InputSettings inputSettings { get { return m_inputSettings; } private set { m_inputSettings = value; } }
        public KB_PlayerBackendSettings playerSettings { get { return m_playerSettings; } private set { m_playerSettings = value; } }
        public KB_PlayerSlotRotation playerSlotRotation { get { return m_playerLookRotation; } private set { m_playerLookRotation = value; } }
        public KB_PlayerItemHandler itemHandler { get { return m_itemHandler; } private set { m_itemHandler = value; } }
        public KB_PlayerKnockbackHandler knockbackHandler { get { return m_knockbackHandler; } private set { m_knockbackHandler = value; } }


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
            m_canUse = true;
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
            if (!m_canUse || !m_isReady || !m_canMove)
                return;
            UpdateRoutine();
        }

        /// <summary>
        /// Update the movement of the player
        /// </summary>
        private void FixedUpdate()
        {
            if (!m_canUse || !m_isReady || !m_canMove)
                return;
            FixedUpdateRoutine();
        }

        /// <summary>
        /// Returns true if the references are valid
        /// </summary>
        private bool CheckComponentReferenceValidity()
        {
            if (m_cachedRigidbody != null & m_cachedSpriteRenderer != null)
                return true;
            else
            {
                new KBLog($"Missing component reference: SpriteRenderer/Rigidbody/PlayerInventory : {m_cachedSpriteRenderer}{m_cachedRigidbody}");
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
            knockbackHandler = new KB_PlayerKnockbackHandler(this, m_cachedRigidbody);
            StartCoroutine(InventorySlotLoader());
        }

        /// <summary>
        /// Slot loading after some buffer time
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
            if (!m_isNetworked)
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
            if (!m_isNetworked)
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
        /// Offline weapon slot rotation update
        /// </summary>
        private void OfflineRotationUpdate()
        {
            m_cachedWeaponSlot.rotation = playerSlotRotation.GetCalculatedRotation();
        }

        /// <summary>
        /// Online weapon slot rotation update
        /// </summary>
        private void OnlineRotationUpdate()
        {
            // Yet to be implemented
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
            // Yet to be implemented
        }        
    }
}
