﻿using UnityEngine;

namespace Knockback.Handlers
{
    /// <summary>
    /// This is the class that runs before all the class begin
    /// </summary>
    public class KB_GameHandler : MonoBehaviour
    {


        private void Start()
        {
            BeginBootstrap();
        }


        /// <summary>
        /// All the initializations happens here
        /// </summary>
        private void BeginBootstrap()
        {
            KB_ResourceHandler.LoadReasourceCollections();
        }

        private void PreLaunchRoutines()
        {

        }

        private void PostLaunchRoutines()
        {

        }

       // private void
    }
}
