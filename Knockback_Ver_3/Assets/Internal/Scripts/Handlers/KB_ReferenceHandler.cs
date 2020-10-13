using Knockback.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Handlers
{
    public static class KB_ReferenceHandler
    {
        public class ObjectContainer
        {
            public ObjectContainer(Object value, string tag) { this.value = value; this.tag = tag; }

            private Object value;
            private string tag;

            public Object GetValue() { return value; }
            public string GetTag() { return tag; }
            public bool IsEqual(Object value) { return this.value == value; }
            public bool IsEqual(string tag) { return this.tag == tag; }
        }

        private static List<ObjectContainer> container = new List<ObjectContainer>();

        /// <summary>
        /// Use this function to add object reference to the world reference handler
        /// </summary>
        /// <param name="value">The class reference you need to store</param>
        public static void Add(Object value) { container.Add(new ObjectContainer(value, value.ToString())); }

        /// <summary>
        /// Use this function to add object reference to the world reference handler
        /// </summary>
        /// <param name="value">The class reference you need to store</param>
        public static void Add(Object value, string tag) { container.Add(new ObjectContainer(value, tag)); }

        /// <summary>
        /// Removes reference of the given value
        /// </summary>
        /// <param name="value">The class reference you need to remove</param>
        public static void Remove(Object value)
        {
            for (int index = 0; index < container.Count; index++)
            {
                if (container[index].IsEqual(value))
                    container.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes reference of the given value
        /// </summary>
        /// <param name="tag">The tag you want to remove</param>
        public static void Remove(string tag)
        {
            for (int index = 0; index < container.Count; index++)
            {
                if (container[index].IsEqual(tag))
                    container.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes references of the same data type
        /// </summary>
        /// <param name="value"></param>
        public static void RemoveAll<T>() where T : class
        {
            for (int index = 0; index < container.Count; index++)
            {
                if (container[index].GetValue() is T)
                    container.RemoveAt(index);
            }
        }

        /// <summary>
        /// Returns true if the specified data type is found and false otherwise
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Out variable of the required type</param>
        /// <returns></returns>
        public static bool GetReference<T>(out T value) where T : class
        {
            value = null;
            foreach (var temp in container)
            {
                if (temp.GetValue() is T)
                {
                    value = temp.GetValue() as T;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the specified data type is found and false otherwise
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Out array variable of the required type</param>
        /// <returns></returns>
        public static bool GetReferences<T>(out List<T> value) where T : class
        {
            bool flag = false;
            List<T> tempArray = new List<T>();

            foreach (var temp in container)
            {
                if (temp.GetValue() is T)
                {
                    tempArray.Add(temp.GetValue() as T);
                    flag = true;
                }
            }
            value = tempArray;
            return flag;
        }

        /// <summary>
        /// Returns true if the tag matches to the reference tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">String value that is linked to the data</param>
        /// <param name="value">Out parameter of the required reference</param>
        /// <returns></returns>
        public static bool GetReference<T>(string tag, out T value) where T : class
        {
            bool flag = false;
            value = null;

            foreach (var temp in container)
            {
                if (temp.IsEqual(tag))
                {
                    value = temp.GetValue() as T;
                    flag = true;
                    break;
                }
            }
            return flag;
        }
    }
}
