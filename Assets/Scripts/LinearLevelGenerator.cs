using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LinearLevelGenerator : MonoBehaviour
{

    public GameObject start;
    public GameObject player;
    public GameObject[] prefabs;
    private Queue<GameObject> currentlyGenerated;
    private float currentLocation;
    private float currentElevation;
    public const int NUM_GENERATED_CHUNKS = 3;
    private const float DEATH_DISTANCE = 100;

    // Start is called before the first frame update
    void Start()
    {
        currentlyGenerated = new Queue<GameObject>();
        currentlyGenerated.Enqueue(start);

        currentLocation = start.gameObject.GetComponent<PieceInfo>().length;
        currentElevation = start.gameObject.GetComponent<PieceInfo>().elevationChange;

        for (int i = 0; i < NUM_GENERATED_CHUNKS * 2; i++)
            generateNewChunk();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        float xDistance = currentlyGenerated.Peek().transform.position.x;
        foreach (GameObject o in currentlyGenerated)
        {
            xDistance += o.gameObject.GetComponent<PieceInfo>().length;
            i++;
            if (i >= NUM_GENERATED_CHUNKS)
                break;
        }
        if (player.transform.position.x > xDistance)
        {
            Destroy(currentlyGenerated.Dequeue());
            generateNewChunk();
        }

        checkDeath();
    }

    private void generateNewChunk()
    {
        int index = Random.Range(0, prefabs.Length);
        currentlyGenerated.Enqueue(Instantiate(prefabs[index], new Vector3(currentLocation, currentElevation, 0), Quaternion.identity));
        currentLocation += prefabs[index].gameObject.GetComponent<PieceInfo>().length;
        currentElevation += prefabs[index].gameObject.GetComponent<PieceInfo>().elevationChange;
    }

    private void checkDeath()
    {
        if (player.transform.position.y < currentElevation - DEATH_DISTANCE)
        {
            GameObject.Find("Canvas").GetComponent<PauseScreen>().die();
        }
    }
}
