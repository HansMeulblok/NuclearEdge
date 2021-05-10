using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject checkPointChunk;
    [SerializeField] private List<GameObject> chunks = new List<GameObject>();
    [SerializeField] private GameObject finishChunk;
    [SerializeField] private Transform chunkHolder;
    [SerializeField] private GameObject checkpoints;

    private bool init = true;

    private float checkpointWidth = 24;
    private float chunkWidth = 110;

    public void SpawnNewChunks(Vector2 position)
    {
        if(init)
        {
            //grab de initial chunks as they are
            foreach (Transform chunk in chunkHolder)
            {
                chunks.Add(transform.gameObject);
            }
            //only do this once
            init = false;
        }
        

        if(chunks != null)
        {
            GameObject chunk = chunks[Random.Range(0, chunks.Count)];
            Instantiate(chunk, position + new Vector2(checkpointWidth, 0), Quaternion.identity, chunkHolder);
            chunks.Remove(chunk);

            if(chunks.Count == 0)
            {
                //instantiate the finish if no more chunks left
                GameObject finish = Instantiate(finishChunk, position + new Vector2(chunkWidth + checkpointWidth, 0), Quaternion.identity, chunkHolder);
                finish.GetComponentInChildren<Checkpoint>().shouldGenerate = false;

            }
            else
            {
                GameObject newCheckPoint = Instantiate(checkPointChunk, position + new Vector2(chunkWidth + checkpointWidth, 0), Quaternion.identity, checkpoints.transform);
                newCheckPoint.GetComponentInChildren<Checkpoint>().shouldGenerate = true;
            }
            
        }
    }
}
