using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;
using Knockback.Handlers;

namespace Knockback.Core
{
    /// <summary>
    /// Generic level manager class for handling all routines in a target scene therefore, this class should be 
    /// inherited inorder to make it custom for each scene
    /// </summary>
    public class KB_LevelManagerCore : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Level manager backend settings")]
        [Scene]
        [SerializeField] protected string m_targetScene;
        [SerializeField] protected string m_levelManagerName;
        [SerializeField] protected LoadSceneMode m_sceneLoadMode;

        //** --PRIVATE ATTRIBUTES--

        private bool m_isSceneLoaded = false;
        private bool m_canLoadExternally = false;
        private bool m_canUnloadExternally = false;
        private AsyncOperation m_loadOperation = null;
        private AsyncOperation m_unloadOperation = null;

        //** --PROTECTED REFERENCES--

        protected bool m_shouldInvoke => m_isSceneLoaded;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to create an instance from the prefab
        /// </summary>
        public virtual KB_LevelManagerCore InstantiateManager()
        {
            if (KB_ReferenceHandler.CheckIfReferenceExists(m_levelManagerName))
                return KB_ReferenceHandler.GetReference(m_levelManagerName) as KB_LevelManagerCore;

            GameObject tempInstance = Instantiate(gameObject);
            tempInstance.name = m_levelManagerName;
            KB_ReferenceHandler.Add(tempInstance.GetComponent<KB_LevelManagerCore>(), m_levelManagerName);

            return tempInstance.GetComponent<KB_LevelManagerCore>();
        }

        /// <summary>
        /// Call this method first to loadup the manager
        /// </summary>
        /// <param name="delayInSeconds">Add delay if you want the scene to be loaded after some time</param>
        /// <param name="shouldLoadLevel">Set as false if you want to load the level manually</param>
        /// <returns></returns>
        public virtual void LoadManager(float delayInSeconds = -1, bool shouldLoadLevel = true)
        {
            m_canLoadExternally = !shouldLoadLevel;

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            if (shouldLoadLevel)
                LoadScene(delayInSeconds);
        }

        /// <summary>
        /// Call this method to unload/destroy this LevelManager
        /// </summary>
        public virtual void UnloadManager(float delayInSeconds = -1, bool shouldUnload = true, bool shouldDestroy = true)
        {
            m_canUnloadExternally = !shouldUnload;

            if (shouldDestroy)
            {
                UnloadScene(delayInSeconds);
                KB_ReferenceHandler.Remove(m_levelManagerName);
                StartCoroutine(LateDeactivator(true));
            }
            else
            {
                if (shouldUnload)
                    UnloadScene(delayInSeconds);
                StartCoroutine(LateDeactivator());
            }
        }

        /// <summary>
        /// Call this method to load the scene externally
        /// </summary>
        public void LoadSceneExternally(float delayInSeconds = -1)
        {
            if (m_canLoadExternally == false)
                return;
            LoadScene(delayInSeconds);
        }

        /// <summary>
        /// Call this method to unload scene externally
        /// </summary>
        public void UnloadSceneExternally(float delayInSeconds = -1)
        {
            if (m_canUnloadExternally == false)
                return;
            UnloadScene(delayInSeconds);
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Awake runs as soon as this gameObject is spawned
        /// </summary>
        private void Awake() => CallOnAwake();

        /// <summary>
        /// Start also runs when this gameObject is spawned
        /// </summary>
        private void Start() => CallOnStart();

        /// <summary>
        /// Update only runs after the target scene is loaded
        /// </summary>
        private void Update()
        {
            if (m_shouldInvoke)
                CallOnUpdate();
        }

        /// <summary>
        /// FixedUpdate only runs after the target scene is loaded
        /// </summary>
        private void FixedUpdate()
        {
            if (m_shouldInvoke)
                CallOnFixedUpdate();
        }

        /// <summary>
        /// Coroutine that loads the scene after sometime
        /// </summary>
        /// <param name="seconds">Time in seconds after whihc the scene has to be loaded</param>
        private IEnumerator DelayedSceneLoader(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            LoadScene();
        }

        /// <summary>
        /// Coroutine that unloads the scene after sometime
        /// </summary>
        /// <param name="seconds">Time in seconds after whihc the scene has to be loaded</param>
        private IEnumerator DelayedSceneUnloader(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            UnloadScene();
        }

        /// <summary>
        /// Coroutine checks if the scene has loaded or not
        /// </summary>
        /// <param name="operation">Async operation handle</param>
        /// <param name="isLoadingOperation">Set as true for loading and false for otherwise</param>
        private IEnumerator SceneLoadChecker(AsyncOperation operation, bool isLoadingOperation = true)
        {
            yield return new WaitUntil(() => operation.isDone);
            if (isLoadingOperation)
                m_isSceneLoaded = true;
            else
                m_isSceneLoaded = false;
        }

        /// <summary>
        /// Coroutine to deactivate this gameObject after the level is unloaded
        /// </summary>
        private IEnumerator LateDeactivator(bool shouldDestroy = false)
        {
            yield return new WaitUntil(() => !m_isSceneLoaded);
            yield return null;

            if (shouldDestroy)
                Destroy(gameObject, 1);
            else
                gameObject.SetActive(false);
        }

        /// <summary>
        /// Loads the scene this amanger handles
        /// </summary>
        private void LoadScene(float delayInSeconds = -1)
        {
            if (m_isSceneLoaded == true)
                return;
            if (m_targetScene.Length == 0)
                return;

            if (delayInSeconds > 0)
                StartCoroutine(DelayedSceneLoader(delayInSeconds));
            else
            {
                if (m_loadOperation != null)
                    m_loadOperation.completed -= CallOnSceneLoad;
                m_loadOperation = SceneManager.LoadSceneAsync(m_targetScene, m_sceneLoadMode);
                m_loadOperation.completed += CallOnSceneLoad;
                StartCoroutine(SceneLoadChecker(m_loadOperation));
            }
        }

        /// <summary>
        /// To unload the scene this manager handles
        /// </summary>
        private void UnloadScene(float delayInSeconds = -1)
        {
            if (m_isSceneLoaded == false)
                return;
            if (m_targetScene.Length == 0)
                return;

            if (delayInSeconds > 0)
                StartCoroutine(DelayedSceneUnloader(delayInSeconds));
            else
            {
                if (m_unloadOperation != null)
                    m_unloadOperation.completed -= CallOnSceneUnload;
                m_unloadOperation = SceneManager.UnloadSceneAsync(m_targetScene);
                m_unloadOperation.completed += CallOnSceneUnload;
                StartCoroutine(SceneLoadChecker(m_unloadOperation, false));
            }
        }

        //** --PROTECTED VIRTUAL METHODS--

        /// <summary>
        /// Override these basic methods to use them
        /// </summary>
        protected virtual void CallOnAwake() { }
        protected virtual void CallOnStart() { }
        protected virtual void CallOnUpdate() { }
        protected virtual void CallOnFixedUpdate() { }
        protected virtual void CallOnSceneLoad(AsyncOperation operation) { }
        protected virtual void CallOnSceneUnload(AsyncOperation operation) { }
    }
}