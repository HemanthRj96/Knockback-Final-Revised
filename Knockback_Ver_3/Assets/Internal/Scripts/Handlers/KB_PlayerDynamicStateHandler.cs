using UnityEngine;
using System.Collections;
using Knockback.Utility;

namespace Knockback.Handlers
{
    public class KB_PlayerDynamicStateHandler : MonoBehaviour, IDamage
    {
        //todo: Requires more refactoring
        //todo: Network synchronization

        internal class DynamicStates
        {
            public DynamicStates() { }

            public DynamicStates(float health = 0, float armour = 0, ArmourTypes armourType = 0)
            {
                this.health += health;
                this.armour += armour;
                this.armourType = armourType;
            }

            public float health;
            public float armour;
            public ArmourTypes armourType;
        }

        private static DynamicStates state = new DynamicStates();

        private readonly float maxHealth = 100;
        private readonly float maxArmour = 100;
        private readonly float _type1_Modifier = 0.3f;
        private readonly float _type2_Modifier = 0.5f;
        private readonly float _type3_Modifier = 0.7f;


        public void ApplyDamage(float damage)
        {
            if (state.armourType == ArmourTypes.DefaultNull)
                state = new DynamicStates(-damage);
            else
            {
                switch (state.armourType)
                {
                    case ArmourTypes.type_1:
                        {
                            float finalHealth = (1 - _type1_Modifier) * damage;
                            float finalArmour = _type1_Modifier * damage;
                            state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                    case ArmourTypes.type_2:
                        {
                            float finalHealth = (1 - _type2_Modifier) * damage;
                            float finalArmour = _type2_Modifier * damage;
                            state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                    case ArmourTypes.type_3:
                        {
                            float finalHealth = (1 - _type3_Modifier) * damage;
                            float finalArmour = _type3_Modifier * damage;
                            state = new DynamicStates(finalHealth, finalArmour);
                        }
                        break;
                }
            }
        }

        public void RemoveDamage(float damage)
        {
            state = new DynamicStates(damage);
        }

        public void SetBaseValues()
        {
            state = new DynamicStates(maxHealth, maxArmour, 0);
        }
    }
}