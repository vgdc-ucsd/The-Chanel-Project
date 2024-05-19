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

public class MapGenerator : MonoBehaviour
{
    [Header("Debugging Info")]
    [SerializeField] MapInfo mapInfo;
    public List<Vector3> Points;
    // public List<Vector3> StairsPoints = new();
    // public List<GameObject> stairDirections;

    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    public List<GameObject> allNodes = new();
    public List<List<GameObject>> paths = new();
    [SerializeField] GameObject contents;

    [Header("Prefabs")]
    public GameObject start;
    public GameObject exit;
    public GameObject Encounter;
    public GameObject Shop;
    public GameObject Event;
    public GameObject Boss;
    // public GameObject horStairs;
    // public GameObject diagStairs;
    public GameObject stair;

    [Header("Map Settings")]
    public Vector3 OrientingPosition;
    public Vector3 offsetPosition = new Vector3(-300, -100, 0);
    public Vector3 horStairOffset;
    public Vector3 diagStairOffset;
    public int numberOfRooms;
    // Higher pathDensityIndex means more paths are generated
    public int pathDensityIndex;
    public string KeepTag = "UsedNodes";
    [SerializeField] float heightBetweenRows = 86.6f;
    [SerializeField] float distanceBetweenNodes = 100f;
    [SerializeField] float XOffsetDistanceBetweenRows = 50f;
    public List<MapLayerOptions> layerOptions;

    private List<List<Vector3>> layers;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Sets starting position of character
        OrientingPosition = new Vector2(start.GetComponent<RectTransform>().localPosition.x, start.GetComponent<RectTransform>().localPosition.y);

        // Instantiate start node
        GameObject startObj = Instantiate(start, Vector2.zero, Quaternion.identity, contents.transform);
        startObj.GetComponent<MapNode>().row = 1;

        layers = new(numberOfRooms - 1);
        for (int i = 0; i < layers.Capacity; i++)
        {
            layers.Add(new());
        }

        row1.Add(startObj);

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

        // Store all nodes in one list
        InitializeAllNodesList();

        InstantiateStairs();

        // removes any unused nodes generated
        GameObject[] unusedObjects = GameObject.FindGameObjectsWithTag("NotUsed");
        foreach (GameObject node in unusedObjects)
        {
            Destroy(node);
        }

        // move all nodes and lines by offset position
        GameObject[] usedObjects = GameObject.FindGameObjectsWithTag("UsedNodes");
        foreach (GameObject o in usedObjects)
        {
            o.GetComponent<RectTransform>().localPosition += offsetPosition;
        }

        // MapInfo mapInfo = ScriptableObject.CreateInstance<MapInfo>();
        List<MapNodeType> allMapNodes = new();
        List<int> mapNodeRows = new();
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].GetComponent<MapNode>() != null)
            {
                allMapNodes.Add(allNodes[i].GetComponent<MapNode>().mapNodeType);
                mapNodeRows.Add(allNodes[i].GetComponent<MapNode>().row);
            }
        }

        mapInfo.allNodes = allMapNodes;
        mapInfo.nodeRows = mapNodeRows;
    }

    public void LoadMap(MapInfo mapInfo)
    {
    }

    void InitializeAllNodesList()
    {
        foreach (var path in paths)
        {
            foreach (var node in path)
            {
                if (!allNodes.Contains(node))
                {
                    allNodes.Add(node);
                }
            }
        }
    }

    void MoveBossAndExit()
    {
        Vector2 exitPos = new Vector3(OrientingPosition.x + (2 * XOffsetDistanceBetweenRows) + distanceBetweenNodes * numberOfRooms, OrientingPosition.y + (2 * heightBetweenRows));
        GameObject exitObj = Instantiate(exit, exitPos, Quaternion.identity, contents.transform);

        Vector2 bossPos = new Vector3(OrientingPosition.x + distanceBetweenNodes * numberOfRooms, OrientingPosition.y + (2 * heightBetweenRows));
        GameObject bossObj = Instantiate(Boss, bossPos, Quaternion.identity, contents.transform);
        bossObj.GetComponent<MapNode>().nextNodes.Add(exitObj.GetComponent<MapNode>());

        row3.Add(bossObj);
        row3.Add(exitObj);
        bossObj.tag = "UsedNodes";
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
        GameObject clone = Instantiate(stairsType, point, stairsType.transform.rotation, contents.transform);
        clone.tag = "UsedNodes";
    }
    // Instantiates stairs at point
    void SortNodeRow(Vector3 point, GameObject type)
    {
        if (point.y == OrientingPosition.y)
        {
            GameObject instantiated = Instantiate(type, point, transform.rotation, contents.transform);
            row1.Add(instantiated);
            instantiated.GetComponent<MapNode>().row = 1;
        }
        else if (point.y == OrientingPosition.y + heightBetweenRows)
        {
            GameObject instantiated = Instantiate(type, point, transform.rotation, contents.transform);
            row2.Add(instantiated);
            instantiated.GetComponent<MapNode>().row = 2;
        }
        else if (point.y == OrientingPosition.y + (2 * heightBetweenRows))
        {
            GameObject instantiated = Instantiate(type, point, transform.rotation, contents.transform);
            row3.Add(instantiated);
            instantiated.GetComponent<MapNode>().row = 3;
        }
    }
    // Sorts nodes into rows to draw lines
    void CreatePath(List<GameObject> pathNodes, int currentPathIndex)
    {
        bool repeat = true;
        while (repeat == true)
        {
            pathNodes.Clear();
            pathNodes.Add(row1[0]);
            for (int i = 1; i < numberOfRooms + 2; i++)
            {
                float random = UnityEngine.Random.Range(0f, 1f);
                GameObject lastNode = pathNodes[i - 1];
                Debug.Log(lastNode);
                Debug.Log(lastNode.GetComponent<MapNode>().row);
                if (lastNode.GetComponent<MapNode>().row == 1)
                {
                    if (lastNode == row1[0])
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
                else if (lastNode.GetComponent<MapNode>().row == 2)
                {
                    if (random < 0.5f && i < numberOfRooms + 1)
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
                        if (!pathNodes[i].GetComponent<MapNode>().nextNodes.Contains(pathNodes[i + 1].GetComponent<MapNode>()))
                        {
                            pathNodes[i].GetComponent<MapNode>().nextNodes.Add(pathNodes[i + 1].GetComponent<MapNode>());
                        }

                        if (!pathNodes[i + 1].GetComponent<MapNode>().prevNodes.Contains(pathNodes[i + 1].GetComponent<MapNode>()))
                        {
                            pathNodes[i + 1].GetComponent<MapNode>().prevNodes.Add(pathNodes[i].GetComponent<MapNode>());
                        }
                    }

                }
            }
        }
    }
    // Creates paths to generate stairs between nodes; paths cannot be entirely identical
    /*
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
    */

    // Creates list of stair points to generate them with no copies in the same spot
    /*
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
    */

    void InstantiateStairs()
    {
        foreach (var node in allNodes)
        {
            MapNode mapNode = node.GetComponent<MapNode>();
            foreach (var nextNode in mapNode.nextNodes)
            {
                float yDist = nextNode.row == mapNode.row ? 0f : heightBetweenRows;
                float zAngle = Mathf.Atan2(yDist, XOffsetDistanceBetweenRows) * Mathf.Rad2Deg;
                Vector3 angle = new Vector3(0, 0, zAngle);
                GameObject stairObj = Instantiate(stair, node.transform.position, Quaternion.Euler(angle), node.transform);
                if (nextNode.row == mapNode.row)
                {
                    stairObj.GetComponent<RectTransform>().localPosition = horStairOffset;
                }
                else
                {
                    stairObj.GetComponent<RectTransform>().localPosition = diagStairOffset;
                }
            }
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
}
