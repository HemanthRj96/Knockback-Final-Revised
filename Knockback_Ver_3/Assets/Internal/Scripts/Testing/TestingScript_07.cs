using UnityEngine;
using Knockback.Utility;

namespace Knockback.Testing
{
    public class TestingScript_07 : MonoBehaviour, IUIAction
    {
        public void DoAction(IMessage _message = null)
        {
            Debug.Log("Doing action");
        }
    }
}