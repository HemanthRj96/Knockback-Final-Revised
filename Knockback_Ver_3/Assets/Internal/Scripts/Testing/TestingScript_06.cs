using Knockback.Core;
using Knockback.Handlers;
using UnityEngine;

namespace Knockback.Testing
{
    public class TestingScript_06 : MonoBehaviour
    {
        public KB_LevelManagerCore prefabReference;
        public KB_LevelManagerCore instanceReference;

        private void Start()
        {
            instanceReference = prefabReference?.InstantiateManager();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                instanceReference?.LoadManager();
            if (Input.GetKeyDown(KeyCode.Delete))
                instanceReference?.UnloadManager(0, true, false);
        }
    }
}