using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Utility
{
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

        public static int GenerateId<T>(this T obj, int length = 5)
        {
            length = Mathf.Clamp(length, 3, 8);
            int minValue = (int)Mathf.Pow(10, length - 1);
            int maxValue = (minValue * 10) - 1;
            return Random.Range(minValue, maxValue);
        }

        public static LayerMask CreateLayerMask(this LayerMask layerMask, int[] layers, bool blockOrIgnore = false)
        {
            foreach (int i in layers)
                layerMask = 1 << i;
            if (blockOrIgnore == true)
                layerMask = ~layerMask;
            return layerMask;
        }
    }
}