using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Knockback.Helpers
{
    public class KB_ButtonAnimator : MonoBehaviour
    {
        [SerializeField]private Button m_button= null;

        private void Awake()
        {
            if (m_button == null)
                TryGetComponent(out m_button);
            
            
        }

        private void TestFunction()
        {
            //Debug.Log("Hello there");
        }

    }
}