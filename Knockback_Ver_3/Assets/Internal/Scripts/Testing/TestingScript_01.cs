using UnityEngine;


public class TestingScript_01 : MonoBehaviour
{

    public void Construct(TestingScript_02 test)
    {
        this.test = test;
    }

    TestingScript_02 test = null;


    private void Start()
    {
        Debug.Log($"{test.someString} : {test.someFloat}");
    }
}
