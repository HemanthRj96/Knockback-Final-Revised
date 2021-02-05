using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Knockback.Utility;
using System.Collections.Generic;

namespace Knockback.Core
{
    public class KB_LevelLoaderCore : MonoBehaviour
    {
        //todo: Network implementation part
        //todo: Should refactor the code - KB_LevelLoaderCore

        //** --SERIALIZED ATTRIBUTES--

        [Header("Level Manager Settings")]
        [Space]
        [SerializeField] protected LevelNames m_targetLevel;
        [SerializeField] protected LoadSceneMode m_targetLoadSceneMode = LoadSceneMode.Single;
        [SerializeField] protected List<KB_LevelLoaderCore> m_forwardNavigation = new List<KB_LevelLoaderCore>();
        [SerializeField] protected List<KB_LevelLoaderCore> m_backwardNavigation = new List<KB_LevelLoaderCore>();

        //** --PUBLIC ATTRIBUTES--

        [HideInInspector]
        public bool m_isLevelLoaded { get; protected set; }

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Used to load the target level in the forward direction from the current level
        /// </summary>
        /// <param name="levelIndex">Corresponding integer value of the level enum</param>
        public AsyncOperation NavigateForward(int levelIndex) { return GetTargetForwardNavigation((LevelNames)levelIndex); }

        /// <summary>
        /// Used to load the target level in the backward direction from the current level
        /// </summary>
        /// <param name="levelIndex">Corresponding integer value of the level enum</param>
        public AsyncOperation NavigateBackward(int levelIndex) { return GetTargetBackwardNavigation((LevelNames)levelIndex); }

        public void NavigateForwardForButtons(int levelIndex) { GetTargetForwardNavigation((LevelNames)levelIndex); }
        public void NavigateBackwardForButtons(int levelIndex) { GetTargetBackwardNavigation((LevelNames)levelIndex); }


        /// <summary>
        /// Run this function to load a level asynchronously; Usually this function is automatically handled and 
        /// the only use case is when we have to manually load the level
        /// </summary>
        /// <returns></returns>
        public AsyncOperation LoadLevel()
        {

            //todo: Implement crossfader logic here
            //try { CrossFader.instance.StartFade(); }
            //catch (Exception) { Debug.Log("--CROSSFADER NOT INTIALIZED--"); }

            AsyncOperation operation = null;
            try
            {
                if (m_targetLevel == LevelNames.DefaultNull)
                    throw new Exception();
                operation = SceneManager.LoadSceneAsync(m_targetLevel.ToString(), m_targetLoadSceneMode);
            }
            catch (Exception)
            {
                Debug.LogError("--INVALID INITIALIZATION--:LevelManagerBase:LoadLevel():targetLevel");
                return null;
            }
            // Initialize the level
            InitializeOnLoad();
            m_isLevelLoaded = true;
            return operation;
        }

        /// <summary>
        /// Use this function to unload levels. Implement the logic to required before invoking base.BeginUnloadingLevel
        /// </summary>
        /// <returns></returns>
        public virtual AsyncOperation BeginUnloadingLevel()
        {
            return UnloadLevel();
        }

        //** --PROTECTED METHODS--

        /// <summary>
        /// This code runs first even before awake function.
        /// </summary>
        protected virtual void InitializeOnLoad() { SceneManager.sceneUnloaded += OnLevelUnload; }

        /// <summary>
        /// Run this function to unload current level; However this function should be only used if the loading 
        /// of the level is set to additive.
        /// </summary>
        /// <returns></returns>
        protected AsyncOperation UnloadLevel()
        {
            try
            {
                if (m_targetLoadSceneMode != LoadSceneMode.Additive)
                    throw new Exception();
                if (m_targetLevel == LevelNames.DefaultNull)
                    throw new Exception();
                AsyncOperation operation;
                operation = SceneManager.UnloadSceneAsync(m_targetLevel.ToString(), UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                return operation;
            }
            catch (Exception)
            {
                Debug.LogError("--INVALID VALUES--:LevelManagerBase:UnloadLevel():targetLoadSceneMode\\targetLevel");
                return null;
            }
        }

        /// <summary>
        /// Invoke this function automatically or extent the function and call this function using base.OnLevelUnload()
        /// </summary>
        /// <param name="scene"></param>
        protected virtual void OnLevelUnload(Scene scene)
        {
            //todo: Implement crossfader logic here
            //try { CrossFader.instance.EndFade(); }
            //catch (Exception) { Debug.Log("--CROSSFADER NOT INTIALIZED--"); }

            m_isLevelLoaded = false;
            // Do other logic to upon unloading the level
        }

        /// <summary>
        /// Method to load a target level from the current level
        /// </summary>
        /// <param name="levelManager">Object reference of the current LevelManagerBase</param>
        /// <param name="targetLevel">Target level to be loaded</param>
        protected AsyncOperation GetTargetForwardNavigation(LevelNames targetLevel)
        {
            AsyncOperation operation = null;
            foreach (var level in m_forwardNavigation)
            {
                if (level.m_targetLevel == targetLevel)
                {
                    operation = level.LoadLevel();
                    break;
                }
            }
            if (operation == null)
                Debug.LogError("--INVALID FUNCTION PARAMETER--:LevelManagerBase:GetTargetForwardNavigation():targetLevel");
            return operation;
        }

        /// <summary>
        /// Method to load a target level from the current level
        /// </summary>
        /// <param name="levelManager">Object reference of the current LevelManagerBase</param>
        /// <param name="targetLevel">Target level to be loaded</param>
        protected AsyncOperation GetTargetBackwardNavigation(LevelNames targetLevel)
        {
            AsyncOperation operation = null;
            foreach (var level in m_backwardNavigation)
            {
                if (level.m_targetLevel == targetLevel)
                {
                    operation = level.LoadLevel();
                    break;
                }
            }
            if (operation == null)
                Debug.LogError("--INVALID FUNCTION PARAMETER--:LevelManagerBase:GetTargetBackwardNavigation():targetLevel");
            return operation;
        }

        //** --PRIVATE METHODS--


    }
}