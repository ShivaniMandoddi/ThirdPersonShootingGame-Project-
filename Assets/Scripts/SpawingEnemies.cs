using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawingEnemies : MonoBehaviour
{

    public int enemyCount;
    public int radius;
    
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnEnemy(Transform spawingPosition)
    {
        for (int i = 0; i < enemyCount; i++) // Spawing enemies
        {
            
            Vector3 randomPoint= spawingPosition.position + Random.insideUnitSphere * radius; // Taking random postion in certain radius
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas)) // Checking whether the point is on the terrain or not
            {
                GameObject temp = SpawnManager.instance.GetFromPool("Enemy");
                if (temp != null)
                {
                    
                 
                    Vector3 resultPosition = new Vector3(hit.position.x, Terrain.activeTerrain.SampleHeight(hit.position), hit.position.z);

                     temp.transform.position = resultPosition;
                    temp.SetActive(true);
                    //temp.transform.TransformPoint(resultPosition);
                    
                    //Debug.Log(temp.transform.position);
                }
                else
                    i--;
               // Instantiate(enemyPrefab, resultPosition, Quaternion.identity);
            }
            else
                i--;
          
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="SpawnPoint")
        {
            Debug.Log("Trigger entered");
            SpawnEnemy(other.gameObject.transform);
        }
    }
    
   
    
}
