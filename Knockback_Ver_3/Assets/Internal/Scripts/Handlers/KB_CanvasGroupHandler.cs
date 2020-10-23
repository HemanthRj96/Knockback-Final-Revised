using UnityEngine;
using Knockback.Utility;
using System.Collections.Generic;

namespace Knockback.Handlers
{
    //todo: Commenting
    [RequireComponent(typeof(Canvas))]
    public class KB_CanvasGroupHandler : MonoBehaviour
    {
        [Header("UI handler backend settings")]
        [Space]

        [SerializeField] private bool autoActivate = false;
        [SerializeField] private List<_CanvasGroups> targetCanvasGroups = new List<_CanvasGroups>();
        [SerializeField] private List<_ButtonLookup> buttonCollections = new List<_ButtonLookup>();

        private int activeCanvasGroupIndex = -1;

        private void Awake()
        {
            KB_EventHandler.AddEvent("CANVAS_GROUP_HANDLER", CanvasUpdate);
            if (autoActivate)
                InitCanvas();
        }

        private void InitCanvas()
        {
            if (GetActiveCanvas() == null)
                ActivateCanvas(0);
        }

        private void CanvasUpdate(IMessage message)
        {
            UICanvasButtons buttonType;
            buttonType = (UICanvasButtons)message.data;
            foreach (_ButtonLookup button in buttonCollections)
            {
                if (button.buttontype == buttonType)
                {
                    int targetCanvasGroupIndex = GetCanvasIndex(button.targetCanvasGroup);
                    if (targetCanvasGroupIndex == -1)
                        return;
                    if (activeCanvasGroupIndex == -1)
                        activeCanvasGroupIndex = GetActiveCanvasIndex();

                    DeactivateCanvas(activeCanvasGroupIndex);
                    ActivateCanvas(targetCanvasGroupIndex);
                    activeCanvasGroupIndex = targetCanvasGroupIndex;
                }
            }
        }

        private void ActivateCanvas(int canvasGroupIndex) => targetCanvasGroups[canvasGroupIndex].gameObject.SetActive(true);

        private void DeactivateCanvas(int canvasGroupIndex) => targetCanvasGroups[canvasGroupIndex].gameObject.SetActive(false);

        private int GetCanvasIndex(UICanvasGroups canvasGroupType)
        {
            for (int i = 0; i < targetCanvasGroups.Count; i++)
            {
                if (targetCanvasGroups[i].groupType == canvasGroupType)
                    return i;
            }
            Debug.Log("Didn't find the target id");
            return -1;
        }

        private _CanvasGroups GetActiveCanvas()
        {
            foreach (_CanvasGroups canvas in targetCanvasGroups)
                if (canvas.gameObject.activeInHierarchy)
                    return canvas;
            return null;
        }

        private int GetActiveCanvasIndex() => GetCanvasIndex(GetActiveCanvas().groupType);

    }

    [System.Serializable]
    public class _CanvasGroups
    {
        public GameObject gameObject { get { return canvasGroupType.gameObject; } }
        public Canvas canvasGroupType = null;
        public UICanvasGroups groupType;
    }

    [System.Serializable]
    public class _ButtonLookup
    {
        public UICanvasButtons buttontype;
        public UICanvasGroups targetCanvasGroup;
    }
}