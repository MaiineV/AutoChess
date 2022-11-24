using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<GameObject> floors;

    public int width, height;

    void Start()
    {
        Vector3 actualPosition = Vector3.zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Instantiate(floors[Random.Range(0, floors.Count)], actualPosition, Quaternion.identity, transform);
                actualPosition += Vector3.right;
            }
            actualPosition.x = 0;
            actualPosition += Vector3.forward;
        }

        Camera.main.transform.position = new Vector3(width * 0.5f, 20, height * 0.5f);
    }
}
