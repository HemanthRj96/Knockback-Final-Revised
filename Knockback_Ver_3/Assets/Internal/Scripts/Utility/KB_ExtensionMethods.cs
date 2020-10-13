using Mirror;
using UnityEngine;

namespace Knockback.Utils
{
    public static class KB_ExtensionMethods
    {
        public static void WriteThis(this NetworkWriter writer, TestingScript_01 testingVar)
        {
            writer.WriteInt32(testingVar.myValue);
            writer.WriteVector3(testingVar.myVector);
        }

        public static TestingScript_01 ReadThis(this NetworkReader reader)
        {
            TestingScript_01 testObject = ScriptableObject.CreateInstance<TestingScript_01>();

            return testObject.CopyValue(reader.ReadInt32(), reader.ReadVector3());
        }
    }
}