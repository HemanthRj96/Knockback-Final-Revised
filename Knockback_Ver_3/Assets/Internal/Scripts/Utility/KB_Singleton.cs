using UnityEngine;

namespace Knockback.Utility
{
    public class KB_Singleton<T> : MonoBehaviour where T : KB_Singleton<T>
    {
        public static T instance;
        protected bool doNotDestoryOnLoad = true;

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this as T;

            if (doNotDestoryOnLoad)
                DontDestroyOnLoad(this);
        }
    }
}