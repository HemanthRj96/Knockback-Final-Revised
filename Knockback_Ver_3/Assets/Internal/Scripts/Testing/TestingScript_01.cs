using UnityEngine;
using System.Collections;
using Knockback.Utility;
using Knockback.Handlers;
using System.Threading;

public class TestingScript_01 : MonoBehaviour
{

    public float firstListenerTime = 0;
    public float secondListenerTime = 0;
    public float thirdListenerTime = 0;


    private int numberOfInvokes = 0;


    private void Start()
    {
        KB_EventHandler.AddEvent("TestEvent", FirstListener);
        KB_EventHandler.AddEvent("TestEvent", SecondListener);
        KB_EventHandler.AddEvent("TestEvent", ThirdListener);
    }

    private void FirstListener(IMessage message)
    {
        ++numberOfInvokes;
        firstListenerTime += (Time.realtimeSinceStartup - message.timeUntilActivation) * 1000;
        firstListenerTime /= numberOfInvokes;
    }

    private void SecondListener(IMessage message)
    {
        secondListenerTime += (Time.realtimeSinceStartup - message.timeUntilActivation) * 1000;
        secondListenerTime /= numberOfInvokes;
    }

    private void ThirdListener(IMessage message)
    {
        thirdListenerTime += (Time.realtimeSinceStartup - message.timeUntilActivation) * 1000;
        thirdListenerTime /= numberOfInvokes;
    }
}
