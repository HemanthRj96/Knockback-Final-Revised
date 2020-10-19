using Knockback.Controllers;
using Knockback.Core;
using Knockback.Handlers;
using UnityEngine;

namespace Knockback.Derived
{
    [CreateAssetMenu(fileName = "KB_Speedbooster", menuName = "Player Abilities/KB_Speedbooster")]
    public class KB_Speedbooster : KB_AbilityCore
    {
        [Header("Speedbooster backend")]
        [Space]

        [Range(-500, 500)]
        [Tooltip("The value here is in percentage")]
        [SerializeField] private float percentageModifier = 0;
        [SerializeField] private float absoluteValue = 0;
        [SerializeField] private bool usePercentage = false;

        private KB_PlayerController controller = null;
        private float cachedValue = 0;

        protected override void OnStartEffect()
        {
            if (KB_PlayerHandler.instance == null || KB_PlayerHandler.instance.localPlayer == null)
                return;
            controller = KB_PlayerHandler.instance.localPlayer;
            cachedValue = controller.GetSettings().moveSpeed;

            if (usePercentage)
                controller.ModifySettings(new int[] { 0 }, new dynamic[] { cachedValue * percentageModifier });
            else
                controller.ModifySettings(new int[] { 0 }, new dynamic[] { absoluteValue });
        }

        protected override void OnEndEffect()
        {
            controller.ModifySettings(new int[] { 0 }, new dynamic[] { cachedValue });
        }
    }
}