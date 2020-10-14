using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Knockback.Utility;

namespace Knockback.Handlers
{
    public class KB_PoolHandler : KB_Singleton<KB_PoolHandler>
    {

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

        [Header("Pre-made pool defaults")]
        [Space]

        [SerializeField]
        private List<PoolData> poolList = new List<PoolData>();
        private Dictionary<string, Queue<GameObject>> poolCollection = new Dictionary<string, Queue<GameObject>>();

        protected override void Awake() { base.Awake(); }

        /// <summary>
        /// Create a pool automatically from the pool list
        /// </summary>
        public void InitializePool()
        {
            foreach (PoolData pool in poolList)
            {
                Queue<GameObject> tempQueue = new Queue<GameObject>();
                for (int index = 0; index < pool.poolSize; index++)
                {
                    GameObject tempGameObject = Instantiate(pool.poolPrefab, transform);
                    tempGameObject.SetActive(false);
                    tempQueue.Enqueue(tempGameObject);
                }
                poolCollection.Add(pool.tag, tempQueue);
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
            poolList.Add(new PoolData(tag, prefab, size));

            for (int index = 0; index < size; index++)
            {
                GameObject tempGameObject = Instantiate(prefab, transform);
                tempGameObject.SetActive(false);
                tempQueue.Enqueue(tempGameObject);
            }
            poolCollection.Add(tag, tempQueue);
        }

        /// <summary>
        /// Returns a gameObject inside the pool
        /// </summary>
        /// <param name="tag">Tag to identify the target pool</param>
        /// <returns></returns>
        public GameObject GetFromPool(string tag)
        {
            if (!poolCollection.ContainsKey(tag))
            {
                new KBLog("--INVALID TAG--", 1);
                return null;
            }

            GameObject targetObject = poolCollection[tag].Dequeue();
            targetObject.SetActive(true);
            poolCollection[tag].Enqueue(targetObject);
            return targetObject;
        }

        /// <summary>
        /// Call this method to remove a pool
        /// </summary>
        /// <param name="tag">Name of the pool</param>
        public void DestroyPool(string tag)
        {            
            if (poolCollection.ContainsKey(tag))
            {
                foreach(var item in poolCollection[tag])
                {
                    Destroy(item.gameObject);
                }
                poolCollection.Remove(tag);

                for (int index = 0; index < poolList.Count; index++)
                {
                    if (poolList[index].tag == tag)
                    {
                        poolList.RemoveAt(index);
                        break;
                    }
                }
            }
        }
    }
}