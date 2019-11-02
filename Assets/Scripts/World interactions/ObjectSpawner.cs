using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("list of spawned objects length determins maximum number")]
    public GameObject spawnPrefab;
    public GameObject[] spawnedObjects;
    public float spawnInterval;
    Coroutine spawningDelay;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawningDelay == null)
        {
            for( int i = 0; i < spawnedObjects.Length; i ++)
            {
                if (spawnedObjects[i] == null)
                {
                    
                    index = i;
                    spawningDelay = StartCoroutine(SpawnDelay());
                    break;
                }
            }
        }      
    }

    IEnumerator SpawnDelay()
    { 
        yield return new WaitForSeconds(spawnInterval);
        spawnedObjects[index] = Instantiate(spawnPrefab,transform.position, transform.rotation);
        spawningDelay = null;
    }
}
