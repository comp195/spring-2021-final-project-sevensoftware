using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public int NUM_CHUNKS = 20;
    public int DISTANCE_BETWEEN_CHUNKS = 45;

    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, prefabs.Length);
        for(int i = 0; i < NUM_CHUNKS; i++) {
            for(int j = 0; j < NUM_CHUNKS; j++) {
                Instantiate(prefabs[index], new Vector3(i * DISTANCE_BETWEEN_CHUNKS, 50, j * DISTANCE_BETWEEN_CHUNKS), Quaternion.identity);
                index = Random.Range(0, prefabs.Length);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
