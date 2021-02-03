using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Utility
{
    /// <summary>
    /// This class includes all the extension methods
    /// </summary>
    public static class KB_ExtensionMethods
    {
        /// <summary>
        /// Copies the position and rotation of the transform
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="finalTransform"></param>
        public static void CopyPositionAndRotation(this Transform targetTransform, Transform finalTransform)
        {
            targetTransform.position = finalTransform.position;
            targetTransform.rotation = finalTransform.rotation;
        }

        /// <summary>
        /// Returns an array of child transforms
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] GetChildren(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            for (int index = 0; index < transform.childCount; index++) { children.Add(transform.GetChild(index)); }
            return children.ToArray();
        }

        /// <summary>
        /// Returns an randomly generated Id
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="length">Length of the Id</param>
        /// <returns></returns>
        public static int GenerateId<T>(this T obj, int length = 5)
        {
            length = Mathf.Clamp(length, 3, 8);
            int minValue = (int)Mathf.Pow(10, length - 1);
            int maxValue = (minValue * 10) - 1;
            return Random.Range(minValue, maxValue);
        }

        /// <summary>
        /// Create a layer mask
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layers">All the target layers</param>
        /// <param name="ignoreOrBlock">Set true for an ignore mask and false for a blocking mask</param>
        public static LayerMask CreateLayerMask(this LayerMask layerMask, int[] layers, bool ignoreOrBlock = false)
        {
            foreach (int i in layers)
                layerMask = 1 << i;
            if (ignoreOrBlock == true)
                layerMask = ~layerMask;
            return layerMask;
        }

        /// <summary>
        /// Returns a unit direction vector
        /// </summary>
        /// <param name="inputVector">Target vector</param>
        /// <param name="relativeTo">Relative vector</param>
        /// <returns></returns>
        public static Vector2 GetDirectionOfVector(this Vector2 inputVector, Vector2 relativeTo)
        {
            Vector2 difference = relativeTo - inputVector;
            difference.Normalize();
            return difference;
        }

        /// <summary>
        /// Returns a float angle from a unit vector
        /// </summary>
        /// <param name="directionVector"></param>
        /// <returns></returns>
        public static float GetAngleOfRotationFromDirection(this Vector2 directionVector)
        {
            directionVector.Normalize();
            return Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        }
    }
}