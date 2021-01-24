using UnityEngine;
using Knockback.Handlers;

namespace Knockback.Helpers
{
    public class KB_CanvasGroupButtonBinder : MonoBehaviour
    {
        public string m_buttonName;
        public string m_targetEventTag;

        public void OnButtonClick() => KB_EventHandler.Invoke(m_targetEventTag, m_buttonName);
    }
}