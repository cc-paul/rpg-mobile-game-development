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
    private GameObject obj;
    private GameObject objectToSpawn;
    private Transform parent;
    private Queue<GameObject> objectPool;
    private Pool pool;
    private string objName;

    private void Awake() {
        parent = transform;
    }

    private void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        for (int a = 0; a < pools.Count; a++) {
            pool = pools[a];

            objectPool = new Queue<GameObject>();
            objName = "";

            for (int i = 0; i < pool.size; i++) {
                obj = Instantiate(pool.prefab, parent);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

                if (i == 0) {
                    objName = pool.prefab.name.ToString();
                }
            }

            poolDictionary.Add(objName, objectPool);
        }
    }

    public void UnecessaryPoolCall(int size,GameObject currentObject,string objName,GameObject skillContentContainer) {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        objectPool = new Queue<GameObject>();
        
        for (int i = 0; i < size; i++) {
            obj = Instantiate(currentObject, skillContentContainer.transform);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(objName, objectPool);
    }

    public GameObject SpawnFromPool(string tag) {
        objectToSpawn = poolDictionary[tag].Dequeue();
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}