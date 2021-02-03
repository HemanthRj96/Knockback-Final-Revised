using UnityEngine;
using System.Collections.Generic;
using Knockback.Utility;

namespace Knockback.Handlers
{
    //todo: Minor refactoring - KB_PoolHandler

    /// <summary>
    /// Method that handles object pools
    /// </summary>
    public class KB_PoolHandler : KB_Singleton<KB_PoolHandler>
    {
        //** --INTERNAL CLASS--

        [System.Serializable]
        public class PoolData
        {
            public PoolData(string tag, GameObject poolPrefab, int poolSize)
            {
                this.tag = tag;
                this.poolPrefab = poolPrefab;
                this.poolSize = poolSize;
            }

            public string tag;
            public GameObject poolPrefab;
            public int poolSize;
        }

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Pre-made pool defaults")]
        [Space]

        [SerializeField]
        private List<PoolData> m_poolList = new List<PoolData>();
        private Dictionary<string, Queue<GameObject>> m_poolCollection = new Dictionary<string, Queue<GameObject>>();

        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Create a pool automatically from the pool list
        /// </summary>
        public void InitializePool()
        {
            foreach (PoolData pool in m_poolList)
            {
                Queue<GameObject> tempQueue = new Queue<GameObject>();
                for (int index = 0; index < pool.poolSize; index++)
                {
                    GameObject tempGameObject = Instantiate(pool.poolPrefab, transform);
                    tempGameObject.SetActive(false);
                    tempQueue.Enqueue(tempGameObject);
                }
                m_poolCollection.Add(pool.tag, tempQueue);
            }
        }

        /// <summary>
        /// Function to create pool manually
        /// </summary>
        /// <param name="tag">Tag to identify the target pool</param>
        /// <param name="prefab">Target prefab for pool</param>
        /// <param name="size">Total size of the pool</param>
        public void CreatePool(string tag, GameObject prefab, int size)
        {
            Queue<GameObject> tempQueue = new Queue<GameObject>();
            m_poolList.Add(new PoolData(tag, prefab, size));

            for (int index = 0; index < size; index++)
            {
                GameObject tempGameObject = Instantiate(prefab, transform);
                tempGameObject.SetActive(false);
                tempQueue.Enqueue(tempGameObject);
            }
            m_poolCollection.Add(tag, tempQueue);
        }

        /// <summary>
        /// Returns a gameObject inside the pool
        /// </summary>
        /// <param name="tag">Tag to identify the target pool</param>
        /// <returns></returns>
        public GameObject GetFromPool(string tag)
        {
            if (!m_poolCollection.ContainsKey(tag))
            {
                Debug.LogError("--INVALID TAG--");
                return null;
            }

            GameObject targetObject = m_poolCollection[tag].Dequeue();
            targetObject.SetActive(true);
            m_poolCollection[tag].Enqueue(targetObject);
            return targetObject;
        }

        /// <summary>
        /// Returns true if the pool exists already
        /// </summary>
        /// <param name="tag">Pool tag</param>
        public bool DoesPoolExist(string tag) => m_poolCollection.ContainsKey(tag);

        /// <summary>
        /// Call this method to remove a pool
        /// </summary>
        /// <param name="tag">Name of the pool</param>
        public void DestroyPool(string tag)
        {
            if (m_poolCollection.ContainsKey(tag))
            {
                foreach (var item in m_poolCollection[tag])
                {
                    Destroy(item.gameObject);
                }
                m_poolCollection.Remove(tag);

                for (int index = 0; index < m_poolList.Count; index++)
                {
                    if (m_poolList[index].tag == tag)
                    {
                        m_poolList.RemoveAt(index);
                        break;
                    }
                }
            }
        }

        //** --PRIVATE METHODS--

    }
}