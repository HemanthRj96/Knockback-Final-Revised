using UnityEngine;
using System.Collections;
using Mirror;

namespace Knockback.Utils
{
    public class KBLog
    {
        public KBLog(string text, int type = 2)
        {
            switch (type)
            {
                case 0:
                    Debug.Log(text);
                    break;
                case 1:
                    Debug.LogWarning(text);
                    break;
                case 2:
                    Debug.LogError(text);
                    break;
                default:
                    Debug.LogError(text);
                    break;
            }
        }

        ~KBLog() { }
    }
}