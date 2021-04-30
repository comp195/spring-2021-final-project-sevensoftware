using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearLevelGenerator : MonoBehaviour
{

    public GameObject start;
    public GameObject player;
    public GameObject[] prefabs;
    private GameObject[] currentlyGenerated;
    private float currentLocation;
    private float currentElevation;

    // Start is called before the first frame update
    void Start()
    {
        currentLocation = start.gameObject.GetComponent<StartingZoneInfo>().length;
        currentElevation = start.gameObject.GetComponent<StartingZoneInfo>().elevationChange;

        for (int i = 0; i < 10; i++)
            generateNewChunk();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void generateNewChunk()
    {
        int index = Random.Range(0, prefabs.Length);
        Debug.Log(index);
        Instantiate(prefabs[index], new Vector3(currentLocation, currentElevation, 0), Quaternion.identity);
        currentLocation += prefabs[index].gameObject.GetComponent<PieceInfo>().length;
        currentElevation += prefabs[index].gameObject.GetComponent<PieceInfo>().elevationChange;
    }
}
