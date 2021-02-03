using Knockback.Utility;
using UnityEngine;

namespace Knockback.Handlers
{
    //todo: Implementation - KB_GameHandler

    /// <summary>
    /// This is the class that runs before all the class begin
    /// </summary>
    public class KB_GameHandler : KB_Singleton<KB_GameHandler>
    {
        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--


        //** --PRIVATE ATTRIBUTES--

        //** --PUBLIC METHODS--


        //** --PRIVATE METHODS--

        /// <summary>
        /// Do all the bootstrapping process here
        /// </summary>
        private void Start() => BeginBootstrap();


        /// <summary>
        /// All the initializations happens here
        /// </summary>
        private void BeginBootstrap()
        {
            KB_ResourceHandler.LoadReasourceCollections();
        }

        /// <summary>
        /// All the routines before launch
        /// </summary>
        private void PreLaunchRoutines()
        {

        }

        /// <summary>
        /// All the routine after the launch
        /// </summary>
        private void PostLaunchRoutines()
        {

        }
    }
}
