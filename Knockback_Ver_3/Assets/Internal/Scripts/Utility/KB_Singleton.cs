using UnityEngine;

namespace Knockback.Utils
{
    public class KB_Singleton<T> : MonoBehaviour where T : KB_Singleton<T>
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(this);
        }
    }
}