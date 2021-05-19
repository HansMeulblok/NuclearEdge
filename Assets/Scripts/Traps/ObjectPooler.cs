using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictioray;

    // Use this for initialization
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

        poolDictioray = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = PhotonNetwork.InstantiateRoomObject(pool.prefab.name, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictioray.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictioray.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictioray[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictioray[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
