using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBreaking : MonoBehaviour
{
    int blockId = 0;
    void Update()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            blockId++;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            blockId--;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Vector3 forwardHit = hit.point + transform.TransformDirection(Vector3.forward) * 0.001f;
                Debug.Log($"Did Hit at {forwardHit.x},{forwardHit.y},{forwardHit.z}");
                (Chunk, Vector3Int) chunkPos = WorldGenHandler.INSTANCE.WorldPosToChunkPos(forwardHit);
                chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, Registries.AIR);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                Vector3 backwardHit = hit.point - transform.TransformDirection(Vector3.forward) * 0.001f;
                Debug.Log($"Did Hit at {backwardHit.x},{backwardHit.y},{backwardHit.z}");
                (Chunk, Vector3Int) chunkPos = WorldGenHandler.INSTANCE.WorldPosToChunkPos(backwardHit);
                chunkPos.Item1.SetBlock(chunkPos.Item2.x, chunkPos.Item2.y, chunkPos.Item2.z, blockId % 2 == 0 ? Registries.PLANKS : Registries.GLASS);
            }
        }
    }
}
