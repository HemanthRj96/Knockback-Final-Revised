using UnityEngine;
using Knockback.Handlers;

namespace Knockback.Helpers
{
    /// <summary>
    /// This is an auxillary helper class which is only used by CanvasGroupHandling class
    /// </summary>
    public class KB_CanvasGroupButtonBinder : MonoBehaviour
    {
        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

        public string m_buttonName;
        public string m_targetEventTag;

        //** --METHODS--
        //** --PUBLIC METHODS--

        public void OnButtonClick() => KB_EventHandler.Invoke(m_targetEventTag, m_buttonName);
    }
}