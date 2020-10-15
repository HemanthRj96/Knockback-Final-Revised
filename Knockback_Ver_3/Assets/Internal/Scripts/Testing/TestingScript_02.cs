using Knockback.Handlers;
using Knockback.Utility;
using Mirror;
using TMPro;
using UnityEngine;


public class TestingScript_02 : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            KB_EventHandler.Invoke("TestEvent", "HelloWorld", gameObject, Time.realtimeSinceStartup);
    }
}
