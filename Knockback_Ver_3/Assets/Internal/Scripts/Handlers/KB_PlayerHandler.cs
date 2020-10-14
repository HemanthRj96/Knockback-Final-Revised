using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knockback.Controllers;
using Knockback.Utility;
using Mirror.Examples.Additive;
using TMPro;

namespace Knockback.Handlers
{
    //todo: Commenting :: PlayerHandler
    //todo: Network implementation
    //todo: Score and health handling of every single player

    public class KB_PlayerHandler : KB_Singleton<KB_PlayerHandler>
    {
        public KB_CameraController localCameraController { get; set; } = null;
        public KB_PlayerController localPlayer { get; private set; } = null;

        public Transform testSpawnSite;
        public GameObject playerPrefab;
        private bool spawned = false;

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60;
        }

        public KB_PlayerController GetLocalPlayer()
        {
            return localPlayer;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                SpawnPlayer();
            if (Input.GetKeyDown(KeyCode.Keypad0))
                DestroyPlayer();
        }


        private void SpawnPlayer()
        {
            if (spawned)
                return;
            spawned = true;
            GameObject go = Instantiate(playerPrefab, testSpawnSite.position, testSpawnSite.rotation);
            localPlayer = go.GetComponent<KB_PlayerController>();
            localPlayer.isReady = true;
            localPlayer.canMove = true;
            KB_ReferenceHandler.Add(localPlayer);
        }

        private void DestroyPlayer()
        {
            if (localPlayer != null)
                Destroy(localPlayer.gameObject);
            KB_ReferenceHandler.Remove(localPlayer);
            spawned = false;
        }
    }
}
