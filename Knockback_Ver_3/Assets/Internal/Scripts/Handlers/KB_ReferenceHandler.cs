using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Handlers
{
    /// <summary>
    /// Method that keeps and handles all the dereferncing of the objects
    /// </summary>
    public static class KB_ReferenceHandler
    {
        //** --ATTRIBUTES--
        //** --PUBLIC ATTRIBUTES--

        public static Dictionary<string, Object> m_container = new Dictionary<string, Object>();

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Use this function to add object reference to the world reference handler
        /// </summary>
        /// <param name="value">The class reference you need to store</param>
        public static void Add(Object value) => m_container.Add(value.name, value);

        /// <summary>
        /// Use this function to add object reference to the world reference handler
        /// </summary>
        /// <param name="value">The class reference you need to store</param>
        public static void Add(Object value, string tag)
        {
            if (DuplicateCheck(tag))
                return;
            m_container.Add(tag, value);
        }

        /// <summary>
        /// Removes reference of the given value
        /// </summary>
        /// <param name="value">The class reference you need to remove</param>
        public static void Remove(Object value)
        {
            string key = FindTag(value);
            if (key != null)
                m_container.Remove(key);
        }

        /// <summary>
        /// Removes reference of the given value
        /// </summary>
        /// <param name="tag">The tag you want to remove</param>
        public static void Remove(string tag)
        {
            if (m_container.ContainsKey(tag))
                m_container.Remove(tag);
        }

        /// <summary>
        /// Removes references of the same data type
        /// </summary>
        /// <param name="value"></param>
        public static void RemoveAll<T>() where T : class
        {
            foreach (string tag in m_container.Keys)
                if (m_container[tag] is T)
                    m_container.Remove(tag);
        }

        /// <summary>
        /// Return the the object from the container if found and null otherwise
        /// </summary>
        /// <param name="tag">String tag for the camera controller</param>
        /// <returns></returns>
        public static Object GetReference(string tag)
        {
            if (!m_container.ContainsKey(tag))
                return null;
            return m_container[tag];
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
            foreach (string tag in m_container.Keys)
                if (m_container[tag] is T)
                {
                    value = m_container[tag] as T;
                    return true;
                }
            return false;
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
            value = null;
            if (!m_container.ContainsKey(tag))
                return false;
            value = m_container[tag] as T;
            return true;
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
            value = new List<T>();

            foreach (string tag in m_container.Keys)
                if (m_container[tag] is T)
                {
                    value.Add(m_container[tag] as T);
                    flag = true;
                }
            return flag;
        }

        /// <summary>
        /// Returns true if the reference exists
        /// </summary>
        /// <param name="value">Target value</param>
        public static bool CheckIfReferenceExists(Object value) => m_container.ContainsValue(value);

        /// <summary>
        /// Returns true if the reference exists
        /// </summary>
        /// <param name="tag">Target tag</param>
        public static bool CheckIfReferenceExists(string tag) => m_container.ContainsKey(tag);

        //** --PRIVATE METHODS--

        /// <summary>
        /// Returns true if duplicate exists
        /// </summary>
        /// <param name="tag"></param>
        private static bool DuplicateCheck(string tag)
        {
            if (m_container.ContainsKey(tag))
            {
                Debug.LogError($"Found duplicate key :{tag}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the tag for the target value
        /// </summary>
        /// <param name="value">Target value</param>
        private static string FindTag(Object value)
        {
            foreach (var temp in m_container)
                if (temp.Value == value)
                    return temp.Key;
            return null;
        }
    }
}
