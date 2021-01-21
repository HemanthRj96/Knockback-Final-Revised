using UnityEngine;
using System.Collections;
using Knockback.Utility;

namespace Knockback.Testing
{
    public class TestingScript_04 : MonoBehaviour, IDamage
    {
        public float DamageTaken = 0;

        public void ApplyDamage(float damage)
        {
            DamageTaken += damage;
        }

        public void RemoveDamage(float damage)
        {
        }
    }
}