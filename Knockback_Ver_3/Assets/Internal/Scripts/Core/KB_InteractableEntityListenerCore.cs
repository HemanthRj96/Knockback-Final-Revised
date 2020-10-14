using UnityEngine;
using Knockback.Utility;
using Knockback.Handlers;

namespace Knockback.Core
{
    public class KB_InteractableEntityListenerCore : MonoBehaviour, IInteractableEntity
    {
        [Header("Listener backend settings")]
        [Space]

        [SerializeField] protected int listenerId;

        public bool canUse { get; set; } = false;

        private void Awake()
        {
            KB_EventHandler.instance.AddListener("InteractiveItem", Validate);
        }

        private void Validate(int Id)
        {
            if (Id == listenerId)
                OnUse();
        }

        public void OnUse()
        {
            // Implement this onUse
            if (!canUse)
                return;
        }
    }
}