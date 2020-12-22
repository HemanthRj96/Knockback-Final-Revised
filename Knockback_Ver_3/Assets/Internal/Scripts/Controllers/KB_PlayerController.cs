using UnityEngine;
using Knockback.Utility;
using Knockback.Handlers;
using Knockback.Helpers;
using System.Collections;

namespace Knockback.Controllers
{
    public class KB_PlayerController : MonoBehaviour
    {
        #region --Attributes--


        //*** Backend variables ***//
        [Header("---Backend Settings---")]
        [SerializeField] private KB_InputSettings _inputSettings = new KB_InputSettings();
        [SerializeField] private KB_PlayerBackendSettings _playerSettings = new KB_PlayerBackendSettings();
        [SerializeField] private bool canUse = false;
        [SerializeField] private bool useMouseAndKeyboard = true;

        //*** Cached Variables ***//
        [Header("---Cached Components---")]
        public SpriteRenderer cachedSpriteRenderer = null;
        public Rigidbody2D cachedRigidbody = null;
        public Transform cachedWeaponSlot = null;

        private KB_InventoryHandler _inventoryHandler = new KB_InventoryHandler();
        private KB_Locomotion _locomotionHandler = new KB_Locomotion();
        private KB_PlayerLookRotation _playerLookRotation = new KB_PlayerLookRotation();
        private KB_PlayerWeaponHandler _weaponHandler = new KB_PlayerWeaponHandler();


        //*** External class objects ***//
        public KB_InventoryHandler inventoryHandler { get { return _inventoryHandler; } private set { _inventoryHandler = value; } }
        public KB_Locomotion locomotionHandler { get { return _locomotionHandler; } private set { _locomotionHandler = value; } }
        public KB_InputSettings inputSettings { get { return _inputSettings; } private set { _inputSettings = value; } }
        public KB_PlayerBackendSettings playerSettings { get { return _playerSettings; } private set { _playerSettings = value; } }
        public KB_PlayerLookRotation playerLookRotation { get { return _playerLookRotation; } private set { _playerLookRotation = value; } }
        public KB_PlayerWeaponHandler weaponHandler { get { return _weaponHandler; } private set { _weaponHandler = value; } }


        private bool canMove = true;
        private bool isReady = true;
        private bool isNetworked = false;


        #endregion --Attributes--

        #region --Private function--


        private void Awake()
        {
            InitializeObjects();

            if (CheckComponentReferenceValidity())
                canUse = true;
        }

        private void Update()
        {
            if (!canUse || !isReady || !canMove)
                return;
            

            InputUpdate();
            UpdateRotation();
        }

        private void FixedUpdate()
        {
            if (!canUse || !isReady || !canMove)
                return;

            MovementUpdate();
        }

        //private void OnDestroy() => StopAllCoroutines();

        private void InitializeObjects()
        {
            KB_ReferenceHandler.Add(gameObject, "PlayerController");

            inventoryHandler = new KB_InventoryHandler(this);
            locomotionHandler = new KB_Locomotion(this);
            playerSettings = new KB_PlayerBackendSettings();
            playerLookRotation = new KB_PlayerLookRotation(this);
            weaponHandler = new KB_PlayerWeaponHandler(this);

            StartCoroutine(InventorySlotLoader());
        }

        private IEnumerator InventorySlotLoader() {
            yield return new WaitForSeconds(1);
            inventoryHandler.TrySlotLoad();
        }

        private bool CheckComponentReferenceValidity()
        {
            if (cachedRigidbody != null & cachedSpriteRenderer != null & inventoryHandler != null)
                return true;
            else
            {
                new KBLog($"Missing component reference: SpriteRenderer/Rigidbody/PlayerInventory : {cachedSpriteRenderer}{cachedRigidbody}{inventoryHandler}");
                return false;
            }
        }        

        private void UpdateRotation() => cachedWeaponSlot.rotation = playerLookRotation.GetCalculatedRotation();

        public void ModifySettings(int[] index, dynamic[] values) => playerSettings = new KB_PlayerBackendSettings(index, values);

        public KB_PlayerBackendSettings GetSettings() => playerSettings;

        private void InputUpdate()
        {
            if (!isNetworked)
                OfflineUpdate();
            else
                OnlineUpdate();
        }

        private void MovementUpdate()
        {
            locomotionHandler.Move(inputSettings.MovementInput().x);
            locomotionHandler.Jump(inputSettings.JumpInput());
            locomotionHandler.Dash(inputSettings.DashInput());
        }

        private void OfflineUpdate()
        {
            locomotionHandler.Jump(inputSettings.JumpInput());
        }

        private void OnlineUpdate()
        {
            // For networked systems

        }


        #endregion --Private functions--
    }
}
