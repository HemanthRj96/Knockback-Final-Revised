using UnityEngine;
using System.Collections;

namespace Knockback.Core
{
    public class KB_AbilityCore : ScriptableObject
    {
        public void ApplyEffect()
        {
            OnStartEffect();
        }

        public void RemoveEffect()
        {
            OnEndEffect();
        }

        protected virtual void OnStartEffect() {  }

        protected virtual void OnEndEffect() {  }        
    }
}