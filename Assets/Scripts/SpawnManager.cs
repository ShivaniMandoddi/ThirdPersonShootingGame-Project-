using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyObjectPool;
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }
    public List<GameObject> pool = new List<GameObject>();
    public List<ObjectPools> poolItems = new List<ObjectPools>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ObjectPool
{
    public GameObject item;
    public int count;
}
