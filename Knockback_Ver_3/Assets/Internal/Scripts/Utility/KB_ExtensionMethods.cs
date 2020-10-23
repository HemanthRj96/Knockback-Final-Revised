﻿using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Utility
{
    public static class KB_ExtensionMethods
    {
        /// <summary>
        /// Returns a transform child by string name
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childName">Name of the child object</param>
        /// <returns></returns>
        public static Transform GetChild(this Transform transform, string childName)
        {
            Transform[] children = transform.GetChildren();

            for (int index = 0; index < children.Length; index++)
            {
                if (children[index].name == childName)
                    return children[index];
            }
            return null;
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
    }
}