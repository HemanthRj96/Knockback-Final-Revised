using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Knockback.Handlers
{
    public class KB_ResourceHandler : MonoBehaviour
    {
        //todo: Implemtn the correct logic here
        //todo: Redsign the resource file structure

        private static ResourceCollections collectionHandle;
        public static void LoadReasourceCollections() { collectionHandle = new ResourceCollections(); }
        public static ResourceCollections GetResourceCollectionHandle() { return collectionHandle; }
        public static bool resourceCollectionState { get; private set; } = false;

        public class ResourceCollections
        {
            private GameObject playerPrefab = null;
            private Dictionary<string, GameObject> UIObjectCollections = new Dictionary<string, GameObject>();
            private Dictionary<string, ScriptableObject> scriptableObjectCollections = new Dictionary<string, ScriptableObject>();
            private Dictionary<string, Sprite> spriteCollections = new Dictionary<string, Sprite>();
            private Dictionary<string, GameObject[]> spawnPointCollections = new Dictionary<string, GameObject[]>();
            private Dictionary<string, GameObject> gunIconCollections = new Dictionary<string, GameObject>();

            private const int SpawnpointCollectionCount = 1;
            public ResourceCollections() { LoadCollections(); }
            private void LoadCollections()
            {
                int index = 0;
                resourceCollectionState = false;

                // Player loader
                playerPrefab = Resources.Load<GameObject>("GameObjects/Player/BasePlayer");

                // GameObjects loader
                GameObject[] gameObjects = Resources.LoadAll<GameObject>("GameObjects/UI");
                foreach (var gameObject in gameObjects)
                    UIObjectCollections.Add(gameObject.name, gameObject);

                // Scriptable loader
                ScriptableObject[] scriptableObjects = Resources.LoadAll<ScriptableObject>("Scriptables");
                foreach (var scriptableObject in scriptableObjects)
                    scriptableObjectCollections.Add(scriptableObject.name, scriptableObject);

                // Sprite loader
                Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
                foreach (var sprite in sprites)
                    spriteCollections.Add(sprite.name, sprite);

                // Spawnpoint loader
                for (index = 0; index < SpawnpointCollectionCount; index++)
                    spawnPointCollections.Add($"Set_{index}", Resources.LoadAll<GameObject>($"SpawnPoints/Set_{index}"));
                /*
                                // Gun icon loader
                                GameObject[] gunIcons = Resources.LoadAll<GameObject>("GameObjects/Icons");
                                foreach (var gunIcon in gunIcons)
                                    gunIconCollections.Add(gunIcon.name, gunIcon);
                */
                resourceCollectionState = true;
            }

            public GameObject GetPlayerPrefab() { return playerPrefab; }
            public GameObject GetUIObjectFromTag(string tag) { return UIObjectCollections[tag]; }
            public ScriptableObject GetScriptableObjectFromTag(string tag) { return scriptableObjectCollections[tag]; }
            public Sprite GetSpriteFromTag(string tag) { return spriteCollections[tag]; }
            public GameObject[] GetSpawnpointFromSet(string tag) { return spawnPointCollections[tag]; }
            public GameObject GetIconFromTag(string tag) { return gunIconCollections[tag]; }
        }
    }
}