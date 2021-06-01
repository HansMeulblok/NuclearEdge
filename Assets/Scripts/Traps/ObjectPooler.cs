using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviourPun
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

    public Vector2 startPosition = new Vector2(0, -30);

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Invoke("CreatingPools", 2);
        }
        else
        {
            CreatingPools();
        }
    }

    // Use this for initialization
    void CreatingPools()
    {
        poolDictioray = new Dictionary<string, Queue<GameObject>>();
        int viewId = 1;

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GameObject obj = PhotonNetwork.InstantiateRoomObject(pool.tag, startPosition, Quaternion.identity);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                else if (!PhotonNetwork.IsMasterClient)
                {
                    GameObject obj = PhotonView.Find(viewId).gameObject;
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                    viewId++;
                }
            }
            poolDictioray.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (poolDictioray == null) { return null; }

        if (!poolDictioray.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with " + tag + "tag doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictioray[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictioray[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
