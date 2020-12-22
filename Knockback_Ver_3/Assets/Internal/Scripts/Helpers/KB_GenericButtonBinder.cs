using UnityEngine;
using UnityEngine.UI;
using Knockback.Utility;
using Knockback.Handlers;


namespace Knockback.Helpers
{
    [RequireComponent( typeof(Button))]
    public class KB_GenericButtonBinder : MonoBehaviour
    {
        [SerializeField]
        private UICanvasButtons inputType;
        private Button cachedButton =null;

        private void Awake()
        {
            cachedButton = GetComponent<Button>();
            cachedButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick() => KB_EventHandler.Invoke("CANVAS_GROUP_HANDLER", inputType);

    }
}