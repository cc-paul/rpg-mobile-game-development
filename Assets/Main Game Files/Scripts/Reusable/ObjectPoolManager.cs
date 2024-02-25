using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour {
    [System.Serializable]
    public class Pool {
        public GameObject prefab;
        public int size;
    }


    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            string objName = "";

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab,transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

                if (i == 0) {
                    objName = pool.prefab.name.ToString();
                }
            }

            poolDictionary.Add(objName,objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag) {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}