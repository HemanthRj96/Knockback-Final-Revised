using Knockback.Controllers;
using Knockback.Handlers;
using Knockback.Scriptables;
using Knockback.Utility;
using UnityEngine;

namespace Knockback.Derived
{
    [CreateAssetMenu(fileName = "SpeedBooster", menuName = "Scriptables/SpeedBooster")]
    public class KB_Speedbooster : KB_AbilityBlueprint
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Speedbooster backend")]
        [Space]
        [SerializeField] private float percentageModifier = 0;

        //** --PRIVATE ATTRIBUTES--

        private float cachedValue = 0;
        private PlayerBackendSettingType[] settingType = new PlayerBackendSettingType[] { PlayerBackendSettingType.moveSpeed };


        //** --METHODS--
        //** --OVERRIDED METHODS--

        protected override void OnStartAbility()
        {
            cachedValue = controller.GetSettings().moveSpeed;
            controller.ModifySettings(settingType, new dynamic[] { cachedValue * percentageModifier / 100 });
        }

        protected override void OnEndAbility()
        {
            controller.ModifySettings(settingType, new dynamic[] { cachedValue });
        }
    }
}