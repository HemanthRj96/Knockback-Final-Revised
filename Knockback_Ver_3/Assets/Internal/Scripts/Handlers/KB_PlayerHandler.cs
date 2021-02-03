using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;

namespace Knockback.Handlers
{
    //todo: Refactoring required - KB_PlayerHandler
    //todo: Network implementation - KB_PlayerHandler
    //todo: Score and health handling of every single player - KB_PlayerHandler

    /// <summary>
    /// Class to handle the player spawning and handling other data
    /// </summary>
    public class KB_PlayerHandler : KB_Singleton<KB_PlayerHandler>
    {
        //** -- ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

        public Transform m_testSpawnSite;
        public GameObject m_playerPrefab;

        //** --PRIVATE ATTRIBUTES--

        private bool m_spawned = false;

        //** --PUBLIC REFERENCES--

        public KB_CameraController localCameraController { get; set; } = null;
        public KB_PlayerController localPlayer { get; private set; } = null;


        //** --METHODS--
        //** --PUBLIC METHODS--

        //** --PRIVATE METHODS--

        /// <summary>
        /// Function not implemented properly
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                SpawnPlayer();
            if (Input.GetKeyDown(KeyCode.Keypad0))
                KillPlayer();
        }

        /// <summary>
        /// Method to spawn the player and also initialize some data
        /// </summary>
        private void SpawnPlayer()
        {
            if (m_spawned)
                return;
            m_spawned = true;
            GameObject go = Instantiate(m_playerPrefab, m_testSpawnSite.position, m_testSpawnSite.rotation);
            localPlayer = go.GetComponent<KB_PlayerController>();
            // localPlayer.isReady = true;
            // localPlayer.canMove = true;
            KB_ReferenceHandler.Add(localPlayer);
        }

        /// <summary>
        /// Method to kill the player this handler is responsible for
        /// </summary>
        private void KillPlayer()
        {
            if (localPlayer != null)
                Destroy(localPlayer.gameObject);
            KB_ReferenceHandler.Remove(localPlayer);
            m_spawned = false;
        }

        //** --OVERRIDED METHODS---

        /// <summary>
        /// Do this on awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }
    }
}
