using Knockback.Scriptables;
using Knockback.Utility;
using UnityEngine;

namespace Knockback.Derived
{
    /// <summary>
    /// Basic player speed booster script
    /// </summary>
    [CreateAssetMenu(fileName = "SpeedBooster", menuName = "Scriptables/SpeedBooster")]
    public class KB_Speedbooster : KB_AbilityBlueprint
    {
        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Speedbooster backend")]
        [Space]
        [SerializeField] private float m_percentageModifier = 0;

        //** --PRIVATE ATTRIBUTES--

        private float m_cachedValue = 0;
        private PlayerBackendSettingType[] m_settingType = new PlayerBackendSettingType[] { PlayerBackendSettingType.moveSpeed };


        //** --METHODS--
        //** --OVERRIDED METHODS--

        protected override void OnStartAbility()
        {
            m_cachedValue = controller.GetSettings().m_moveSpeed;
            controller.ModifySettings(m_settingType, new dynamic[] { m_cachedValue * m_percentageModifier / 100 });
        }

        protected override void OnEndAbility()
        {
            controller.ModifySettings(m_settingType, new dynamic[] { m_cachedValue });
        }
    }
}