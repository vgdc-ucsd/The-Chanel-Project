using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class MapGrid : MonoBehaviour
{
    [Header("Debugging Info")]
    public List<Vector3> Points;
    public List<Vector3> StairsPoints = new();
    public List<GameObject> stairDirections;

    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    public List<List<GameObject>> paths = new();

    [Header("Prefabs")]
    public GameObject start;
    public GameObject exit;
    public GameObject Encounter;
    public GameObject Shop;
    public GameObject Event;
    public GameObject Boss;
    public GameObject horStairs;
    public GameObject diagStairs;

    [Header("Map Settings")]
    public Vector3 OrientingPosition;
    public int numberOfRooms;
    // Higher pathDensityIndex means more paths are generated
    public int pathDensityIndex;
    public string KeepTag = "UsedNodes";
    public List<MapLayerOptions> layerOptions;
    [SerializeField] float heightBetweenRows = 86.6f;
    [SerializeField] float distanceBetweenNodes = 100f;
    [SerializeField] float XOffsetDistanceBetweenRows = 50f;

    private List<List<Vector3>> layers;

    // Start is called before the first frame update
    void Start()
    {
        // Sets starting position of character
        OrientingPosition = new Vector3(start.transform.position.x, start.transform.position.y);

        layers = new(numberOfRooms - 1);
        for (int i = 0; i < layers.Capacity; i++)
        {
            layers.Add(new());
        }

        row1.Add(start);

        // Creates preset nodes for first room/boss and exits
        CreateEncounter(new Vector3(OrientingPosition.x + distanceBetweenNodes, OrientingPosition.y));
        CreateEncounter(new Vector3(OrientingPosition.x + XOffsetDistanceBetweenRows, OrientingPosition.y + heightBetweenRows));


        // Creates grid of 3 x numberOfRooms nodes to procedurally generate map nodes
        for (int i = row1.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + distanceBetweenNodes * i, OrientingPosition.y);
            layers[i - row1.Count].Add(Points[Points.Count - 1]);
        }
        for (int i = row2.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + XOffsetDistanceBetweenRows + distanceBetweenNodes * i, OrientingPosition.y + heightBetweenRows);
            layers[i - row2.Count].Add(Points[Points.Count - 1]);
        }
        for (int i = row3.Count; i < numberOfRooms - 1; i++)
        {
            AddPoints(OrientingPosition.x + (2 * XOffsetDistanceBetweenRows) + distanceBetweenNodes * i, OrientingPosition.y + (2 * heightBetweenRows));
            layers[i].Add(Points[Points.Count - 1]);
        }
        transform.rotation = Quaternion.identity;

        // Generates Encounter, Shop, or Event for each point aside from preset nodes
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Count; j++)
            {
                GenerateRandomNode(i, layers[i][j]);
            }
        }

        MoveBossAndExit();
        CreateStairs(new Vector3(exit.transform.position.x - XOffsetDistanceBetweenRows, exit.transform.position.y), horStairs);

        // Creates path and stairs
        if (pathDensityIndex < 1)
        {
            Debug.Log("Number of Paths needs to be greater than 0");
        }


        for (int i = 0; i < pathDensityIndex; i++)
        {
            paths.Add(new List<GameObject>());
            CreatePath(paths[i], i);
        }

        InstantiateStairs();

        // removes any unused nodes generated
        GameObject[] unusedObjects = GameObject.FindGameObjectsWithTag("NotUsed");
        foreach (GameObject node in unusedObjects)
        {
            Destroy(node);
        }
    }

    void MoveBossAndExit()
    {
        exit.transform.position = new Vector3(OrientingPosition.x + (2 * XOffsetDistanceBetweenRows) + distanceBetweenNodes * numberOfRooms, OrientingPosition.y + (2 * heightBetweenRows));
        Boss.transform.position = new Vector3(OrientingPosition.x + distanceBetweenNodes * numberOfRooms, OrientingPosition.y + (2 * heightBetweenRows));
        row3.Add(Boss);
        row3.Add(exit);
        exit.tag = "UsedNodes";
    }
    // Sets exit position based on number of rooms and adds it to row3
    void AddPoints(float x, float y)
    {
        Points.Add(new Vector3(x, y));
    }
    // Creates list of Points where nodes will be generated
    void CreateEncounter(Vector3 point)
    {
        SortNodeRow(point, Encounter);
    }
    // Instantiates Encounter at point
    void CreateShop(Vector3 point)
    {
        SortNodeRow(point, Shop);
    }
    // Instantiates Shop at point
    void CreateEvent(Vector3 point)
    {
        SortNodeRow(point, Event);
    }
    // Instantiates Event at point
    void CreateStairs(Vector3 point, GameObject stairsType)
    {
        GameObject clone = Instantiate(stairsType, point, stairsType.transform.rotation, stairsType.transform.parent);
        clone.tag = "UsedNodes";
    }
    // Instantiates stairs at point
    void SortNodeRow(Vector3 point, GameObject type)
    {
        if (point.y == OrientingPosition.y)
        {
            row1.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
        else if (point.y == OrientingPosition.y + heightBetweenRows)
        {
            row2.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
        else if (point.y == OrientingPosition.y + (2 * heightBetweenRows))
        {
            row3.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
    }
    // Sorts nodes into rows to draw lines
    void CreatePath(List<GameObject> pathNodes, int currentPathIndex)
    {
        bool repeat = true;
        while (repeat == true)
        {
            pathNodes.Clear();
            pathNodes.Add(start);
            for (int i = 1; i < numberOfRooms + 2; i++)
            {
                float random = UnityEngine.Random.Range(0f, 1f);
                GameObject lastNode = pathNodes[i - 1];
                int lastNodeRow = GetNodeRow(lastNode);
                if (lastNodeRow == 1)
                {
                    if (lastNode == start)
                    {
                        if (random < 0.5 && i < numberOfRooms)
                        {
                            pathNodes.Add(row1[i]);
                            row1[i].tag = KeepTag;
                        }
                        else
                        {
                            pathNodes.Add(row2[i - 1]);
                            row2[i - 1].tag = KeepTag;
                        }
                    }
                    else
                    {
                        if (i < (numberOfRooms + 2) / 3)
                        {
                            if (random < 0.8)
                            {
                                pathNodes.Add(row1[i]);
                                row1[i].tag = KeepTag;
                            }
                            else
                            {
                                pathNodes.Add(row2[i - 1]);
                                row2[i - 1].tag = KeepTag;
                            }
                        }
                        else
                        {
                            if (random < 0.6 && i < numberOfRooms)
                            {
                                pathNodes.Add(row1[i]);
                                row1[i].tag = KeepTag;
                            }
                            else
                            {
                                pathNodes.Add(row2[i - 1]);
                                row2[i - 1].tag = KeepTag;
                            }
                        }
                    }
                }
                else if (lastNodeRow == 2)
                {
                    if (random < 0.7 && i < numberOfRooms + 1)
                    {
                        pathNodes.Add(row2[i - 1]);
                        row2[i - 1].tag = KeepTag;
                    }
                    else
                    {
                        pathNodes.Add(row3[i - 2]);
                        row3[i - 2].tag = KeepTag;
                    }
                }
                else
                {
                    pathNodes.Add(row3[i - 2]);
                    row3[i - 2].tag = KeepTag;
                }
            }
            if (paths[0].Equals(pathNodes))
            {
                repeat = false;
                for (int i = 0; i < pathNodes.Count - 1; i++)
                {
                    pathNodes[i].GetComponent<MapNode>().nextNodes.Add(pathNodes[i + 1].GetComponent<MapNode>());
                    pathNodes[i + 1].GetComponent<MapNode>().prevNodes.Add(pathNodes[i].GetComponent<MapNode>());
                }
            }
            else
            {
                if (!CheckDuplicatePath(pathNodes, currentPathIndex))
                {
                    repeat = false;
                    for (int i = 0; i < pathNodes.Count - 1; i++)
                    {
                        pathNodes[i].GetComponent<MapNode>().nextNodes.Add(pathNodes[i + 1].GetComponent<MapNode>());
                        pathNodes[i + 1].GetComponent<MapNode>().prevNodes.Add(pathNodes[i].GetComponent<MapNode>());
                    }

                }
            }
        }
    }
    // Creates paths to generate stairs between nodes; paths cannot be entirely identical
    void ListStairPoints(List<Vector3> StairsPoints, List<GameObject> path)
    {
        if (StairsPoints.Count == 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 pos = new((path[i].transform.position.x + path[i + 1].transform.position.x) / 2, (path[i].transform.position.y + path[i + 1].transform.position.y) / 2);
                StairsPoints.Add(pos);
                if (pos.y == path[i].transform.position.y)
                {
                    stairDirections.Add(horStairs);
                }
                else
                {
                    stairDirections.Add(diagStairs);
                }
            }
        }
        else
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                int count = 0;
                Vector3 pos = new((path[i].transform.position.x + path[i + 1].transform.position.x) / 2, (path[i].transform.position.y + path[i + 1].transform.position.y) / 2);
                foreach (Vector3 p in StairsPoints)
                {
                    if (p != pos)
                    {
                        continue;
                    }
                    else
                    {
                        count++;
                        break;
                    }
                }
                if (count == 0)
                {
                    StairsPoints.Add(pos);
                    if (pos.y == path[i].transform.position.y)
                    {
                        stairDirections.Add(horStairs);
                    }
                    else
                    {
                        stairDirections.Add(diagStairs);
                    }
                }
            }
        }
    }

    // Creates list of stair points to generate them with no copies in the same spot
    void InstantiateStairs()
    {
        foreach (var path in paths)
        {
            ListStairPoints(StairsPoints, path);
        }

        for (int i = 0; i < StairsPoints.Count; i++)
        {
            CreateStairs(StairsPoints[i], stairDirections[i]);
        }
    }
    // Generates stairs

    bool CheckDuplicatePath(List<GameObject> path, int currentIndex)
    {
        bool duplicate = false;
        for (int i = 0; i < currentIndex; i++)
        {
            bool dup = true;
            for (int j = 0; j < paths[i].Count; j++)
            {
                dup &= paths[i][j] == path[j];
            }
            duplicate |= dup;
        }

        for (int i = currentIndex + 1; i < paths.Count; i++)
        {
            bool dup = true;
            for (int j = 0; j < paths[i].Count; j++)
            {
                dup &= paths[i][j] == path[j];
            }
            duplicate |= dup;
        }

        return duplicate;
    }

    public void LockSiblingNodes()
    {
        foreach (var path in paths)
        {
            foreach (var node in path)
            {
                node.GetComponent<MapNode>().locked = true;
            }
        }
    }

    private void GenerateRandomNode(int i, Vector3 point)
    {
        // THIS IS FOR DEBUGGING PURPOSES
        if (layerOptions.Count != numberOfRooms - 1)
        {
            Debug.Log("There are unequal amounts of layerOptions and numberOfRooms. layerOptions.Length should be (numberOfRooms - 1).");
        }

        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < layerOptions[i].randomization)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            if (layerOptions[i].mapNodeType == MapNodeType.Encounter)
            {
                if (r < 0.6f)
                {
                    CreateEvent(point);
                }
                else
                {
                    CreateShop(point);
                }
            }
            else if (layerOptions[i].mapNodeType == MapNodeType.Event)
            {
                if (r < 0.6f)
                {
                    CreateEncounter(point);
                }
                else
                {
                    CreateShop(point);
                }
            }
            else
            {
                if (r < 0.6f)
                {
                    CreateEncounter(point);
                }
                else
                {
                    CreateEvent(point);
                }
            }
        }
        else
        {
            if (layerOptions[i].mapNodeType == MapNodeType.Encounter)
            {
                CreateEncounter(point);
            }
            else if (layerOptions[i].mapNodeType == MapNodeType.Shop)
            {
                CreateShop(point);
            }
            else if (layerOptions[i].mapNodeType == MapNodeType.Event)
            {
                CreateEvent(point);
            }
            else
            {
                Debug.Log("Invalid layer node type for layer: " + (i + 2));
            }
        }
    }

    private int GetNodeRow(GameObject node)
    {
        if (node.transform.position.y == OrientingPosition.y)
        {
            return 1;
        }
        else if (node.transform.position.y == OrientingPosition.y + heightBetweenRows)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
