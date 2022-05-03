using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyObjectPool
{
    [System.Serializable]
    public class ObjectPools
    {
        public GameObject prefab;
        public int count;
        
    }
    public class Player
    {
        public int health;
        public int maxhealth=30;
        public int ammo;
        public int maxAmmo=50;
        
        
    }
    
}
