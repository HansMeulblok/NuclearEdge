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

    private float InBetweenChunkWidth = 24;
    private float chunkWidth = 110;

    public void SpawnNewChunks(Vector2 position)
    {
        if(init)
        {
            //grab de initial chunks as they are
            foreach (Transform chunk in chunkHolder)
            {
                chunks.Add(chunk.gameObject);
            }

            //only do this once
            init = false;
        }
        

        if(chunks != null)
        {
            GameObject chunk = chunks[Random.Range(0, chunks.Count)];
            Instantiate(chunk, position + new Vector2(InBetweenChunkWidth, 0), Quaternion.identity, chunkHolder);
            chunks.Remove(chunk);

            if (chunks.Count == 0)
            {
                //instantiate the finish if no more chunks left
                GameObject finish = Instantiate(finishChunk, position + new Vector2(chunkWidth + InBetweenChunkWidth * 2, 0), Quaternion.identity, chunkHolder);

            }
            else
            {
                GameObject inbetweenChunk = Instantiate(checkPointChunk, position + new Vector2(chunkWidth + InBetweenChunkWidth, 0), Quaternion.identity, checkpoints.transform);
                inbetweenChunk.GetComponentInChildren<LevelGenerationTrigger>().shouldGenerate = true;
            }
            
        }
    }
}
