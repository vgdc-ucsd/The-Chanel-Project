using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapCol;

    [HideInInspector] public GameObject startInstace;
    [HideInInspector] public GameObject exitInstace;
    [HideInInspector] public MapNode lastVisitedNode;

    private List<MapNode[]> mapNodes;
    private List<List<MapNode>> paths;
    private const int mapRow = 3;

    void Start()
    {
        PersistentData.Instance.VsState = VsScreenState.VS;

        if (PersistentData.Instance.mapInfo.nodePoints.Count != 0)
        {
            // LoadMap(PersistentData.Instance.mapInfo);
        }
        else
        {
            GenerateMap();
        }
    }


    public void GenerateMap()
    {
        // Choose Starting Point
        int startNodeRow = Random.Range(0, 3);
        mapNodes[0][startNodeRow] = new();
        mapNodes[0][startNodeRow].mapNodeType = MapNodeType.Start;
        mapNodes[0][startNodeRow].point = new(0, startNodeRow);
        for (int i = 0; i < 10; i++)
        {
            // Create Paths
            foreach (MapNode node in mapNodes[i])
            {
                if (node != null)
                    CreatePath(node);
            }
        }

        // Instantiate nodes at correct points
        // Instantiate the connections/lines between nodes

        // Add positional offset?

        // SaveMap();
        // Set startInstace, exitInstance, lastVisitedNode
    }

    private void CreatePath(MapNode node)
    {

    }
}
