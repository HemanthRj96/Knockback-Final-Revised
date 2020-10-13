using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Testing/TestObject")]
public class TestingScript_01 : ScriptableObject
{
    public TestingScript_01 CopyValue(int value, Vector3 myVector)
    {
        this.myValue = value;
        this.myVector = myVector;
        return this;
    }

    public int myValue = 23;
    public Vector3 myVector;
}
