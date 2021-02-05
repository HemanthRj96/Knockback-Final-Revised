using UnityEngine;
using UnityEngine.UI;

namespace Knockback.Helpers
{
    //todo: Implementation - KB_ButtonAnimator

    /// <summary>
    /// Method for animating buttons
    /// </summary>
    public class KB_ButtonAnimator : MonoBehaviour
    {
        [SerializeField] private Button m_button = null;

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