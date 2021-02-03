using UnityEngine;
using Knockback.Utility;

namespace Knockback.Handlers
{
    //todo: Requires more refactoring - KB_PlayerDynamicStateHandler
    //todo: Network synchronization - KB_PlayerDynamicStateHandler
    public class KB_PlayerDynamicStateHandler : IDamage
    {
        //** --INTERNAL CLASS--

        internal class DynamicStates
        {
            public DynamicStates() { }

            public DynamicStates(float health = 0, float armour = 0, ArmourTypes armourType = 0)
            {
                mi_health += health;
                mi_armour += armour;
                mi_armourType = armourType;
            }

            public float mi_health;
            public float mi_armour;
            public ArmourTypes mi_armourType;
        }

        //** --ATTRIBTUES--
        //** --PRIVATE ATTRIBUTES--

        private static DynamicStates m_state = new DynamicStates();
        private readonly float m_maxHealth = 100;
        private readonly float m_maxArmour = 100;
        private readonly float m_type1_Modifier = 0.3f;
        private readonly float m_type2_Modifier = 0.5f;
        private readonly float m_type3_Modifier = 0.7f;
        private GameObject m_recentDamageSource = null;

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Call this method to apply damage to the player
        /// </summary>
        /// <param name="damage">Damage amount</param>
        /// <param name="source">Source of the damage</param>
        public void ApplyDamage(float damage, GameObject source)
        {
            m_recentDamageSource = source;
            if (m_state.mi_armourType == ArmourTypes.DefaultNull)
                m_state = new DynamicStates(-damage);
            else
            {
                switch (m_state.mi_armourType)
                {
                    case ArmourTypes.type_1:
                        {
                            float finalHealth = (1 - m_type1_Modifier) * damage;
                            float finalArmour = m_type1_Modifier * damage;
                            m_state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                    case ArmourTypes.type_2:
                        {
                            float finalHealth = (1 - m_type2_Modifier) * damage;
                            float finalArmour = m_type2_Modifier * damage;
                            m_state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                    case ArmourTypes.type_3:
                        {
                            float finalHealth = (1 - m_type3_Modifier) * damage;
                            float finalArmour = m_type3_Modifier * damage;
                            m_state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Remove damage from the player
        /// </summary>
        /// <param name="damage">Amount of damage to be removed</param>
        /// <param name="source">Source of the damage removal</param>
        public void RemoveDamage(float damage) => m_state = new DynamicStates(damage);

        /// <summary>
        /// Method to set the base values
        /// </summary>
        public void SetBaseValues() => m_state = new DynamicStates(m_maxHealth, m_maxArmour, 0);

        /// <summary>
        /// Returns the last object that inflicted damage
        /// </summary>
        /// <returns></returns>
        public GameObject GetRecentDamageSource() => m_recentDamageSource;
    }
}