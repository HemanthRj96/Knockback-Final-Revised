using UnityEngine;
using System.Collections.Generic;

namespace Knockback.Handlers
{
    /// <summary>
    /// Class handles all the resource files that are loaded during the runtime of the program
    /// </summary>
    public static class KB_ResourceHandler
    {

        //** --INTERNAL CLASS--

        public class ResourceCollections
        {
            public ResourceCollections() { LoadCollections(); }

            /// <summary>
            /// Player prefab
            /// </summary>
            private GameObject playerPrefab = null;
            /// <summary>
            /// Collection of enemy prefabs
            /// </summary>
            private Dictionary<string, GameObject> enemyPrefabCollection = new Dictionary<string, GameObject>();
            /// <summary>
            /// Collections of all the sprites in the game
            /// </summary>
            private Dictionary<string, Sprite> spriteCollection = new Dictionary<string, Sprite>();
            /// <summary>
            /// Collection of all the animators used in the game
            /// </summary>
            private Dictionary<string, Animator> animatorCollection = new Dictionary<string, Animator>();
            /// <summary>
            /// Collections of all the scriptable objects in the game / these are not relevant to the gameplay itself
            /// </summary>
            private Dictionary<string, ScriptableObject> scriptableObjectCollection = new Dictionary<string, ScriptableObject>();
            /// <summary>
            /// Collections of all the prefabs in the game
            /// </summary>
            private Dictionary<string, GameObject> prefabCollection = new Dictionary<string, GameObject>();
            /// <summary>
            /// Collection of all the UIObjects
            /// </summary>
            private Dictionary<string, GameObject> UIObjectCollection = new Dictionary<string, GameObject>();
            /// <summary>
            /// Collection of all the Sounds
            /// </summary>
            private Dictionary<string, AudioClip> audioCollection = new Dictionary<string, AudioClip>();
            /// <summary>
            /// Collection of all spawnpoint collections
            /// </summary>
            private Dictionary<string, Dictionary<string, GameObject>> spawnpointCollectionList = new Dictionary<string, Dictionary<string, GameObject>>();

            private const int SpawnpointCollectionCount = 1;

            private void LoadCollections()
            {
                if (m_isResourceLoaded)
                    return;
                //Sprite collection loader
                LoadResourcesFromPath("Sprites", out Sprite[] sprites);
                CreateCollection(sprites, out spriteCollection);
                //Animator collection loader
                LoadResourcesFromPath("Animators", out Animator[] animators);
                CreateCollection(animators, out animatorCollection);
                //ScriptableObject collection loader
                LoadResourcesFromPath("Scriptables", out ScriptableObject[] scriptableObjects);
                CreateCollection(scriptableObjects, out scriptableObjectCollection);
                //Enemy prefab collection loader
                LoadResourcesFromPath("OtherPrefabs/EnemyPrefabs", out GameObject[] otherPrefabs);
                CreateCollection(otherPrefabs, out enemyPrefabCollection);
                //Prefab collection loader
                LoadResourcesFromPath("Prefabs", out GameObject[] prefabs);
                CreateCollection(prefabs, out prefabCollection);
                //UIObjects collection loader
                LoadResourcesFromPath("UIObjects", out GameObject[] UIObjects);
                CreateCollection(UIObjects, out UIObjectCollection);
                //Sound collection loader
                LoadResourcesFromPath("Sounds", out AudioClip[] sounds);
                CreateCollection(sounds, out audioCollection);
                //Spawnpoint collection loader
                for (int index = 0; index < SpawnpointCollectionCount; index++)
                {
                    LoadResourcesFromPath($"Spawnpoints/Set_{index}", out GameObject[] spawnpoints);
                    CreateCollection(spawnpoints, out Dictionary<string, GameObject> spawnpointcollection);
                    spawnpointCollectionList.Add($"Set_{index}", spawnpointcollection);
                }

                m_isResourceLoaded = true;
            }

            private void LoadResourcesFromPath<T>(string path, out T[] outData) where T : class
            {
                List<T> sortedObjects = new List<T>();
                Object[] unsortedEntities = Resources.LoadAll(path);

                foreach (Object entity in unsortedEntities)
                    if (entity is T)
                        sortedObjects.Add(entity as T);

                outData = sortedObjects.ToArray();
            }

            private void CreateCollection<T>(T[] inData, out Dictionary<string, T> outCollection)
            {
                outCollection = new Dictionary<string, T>();
                foreach (T data in inData)
                    outCollection.Add(data.ToString(), data);
            }

            public GameObject GetPlayerPrefab() => playerPrefab;
            public Dictionary<string, Sprite> GetSpriteCollection() => spriteCollection;
            public Dictionary<string, Animator> GetAnimatorCollection() => animatorCollection;
            public Dictionary<string, ScriptableObject> GetScriptableObjectCollection() => scriptableObjectCollection;
            public Dictionary<string, GameObject> GetPrefabCollection() => prefabCollection;
            public Dictionary<string, GameObject> GetEnemyPrefabCollection() => enemyPrefabCollection;
            public Dictionary<string, AudioClip> GetSoundCollection() => audioCollection;
            public Dictionary<string, GameObject> GetUIGameObjectCollection() => UIObjectCollection;
            public Dictionary<string, GameObject> GetSpawnpointCollectionFromSet(string collectionSet) => spawnpointCollectionList[collectionSet];
        }

        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

        public static ResourceCollections m_collectionHandle;
        public static bool m_isResourceLoaded = false;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to laod the resource collections
        /// </summary>
        public static void LoadReasourceCollections() => m_collectionHandle = new ResourceCollections();
    }
}