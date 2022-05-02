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
        AddToPool();  
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    public  void AddToPool()
    {
        foreach (var item in poolItems)
        {
            for (int i = 0; i < item.count; i++)
            {
                GameObject temp=Instantiate(item.prefab);
                
                pool.Add(temp);
                temp.SetActive(false);
            }
        }
    }
    public GameObject GetFromPool(string tagName)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if(pool[i].tag==tagName && !pool[i].activeInHierarchy)
            {
                return (pool[i]);
            }
        }
        return null;
    }
}
