using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class MapGrid : MonoBehaviour
{
    public List<Vector3> Points;
    public List<Vector3> StairsPoints = new();
    public List<GameObject> stairDirections;
    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    public List<List<GameObject>> paths = new();
    public List<GameObject> path1;
    public List<GameObject> path2;
    public List<GameObject> path3;
    public GameObject start;
    public GameObject exit;
    public GameObject Encounter;
    public GameObject Shop;
    public GameObject Event;
    public GameObject Boss;
    public GameObject horStairs;
    public GameObject diagStairs;
    public Vector3 OrientingPosition;
    public int numberOfRooms;
    public string KeepTag = "UsedNodes";

    // Start is called before the first frame update
    void Start()
    {
        // Sets starting position of character
        OrientingPosition = new Vector3(start.transform.position.x, start.transform.position.y);

        row1.Add(start);

        // Creates preset nodes for first room/boss and exits
        CreateEncounter(new Vector3(OrientingPosition.x + 100, OrientingPosition.y));
        CreateEncounter(new Vector3(OrientingPosition.x + 50, OrientingPosition.y + 86.6f));

        // Creates grid of 3 x numberOfRooms points to procedurally generate map nodes
        for (int i = row1.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + 100 * i, OrientingPosition.y);
        }
        for (int i = row2.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + 50 + 100 * i, OrientingPosition.y + 86.6f);
        }
        for (int i = row3.Count; i < numberOfRooms - 1; i++)
        {
            AddPoints(OrientingPosition.x + 100 * (i + 1), OrientingPosition.y + 173.2f);
        }
        transform.rotation = Quaternion.identity;

        // Generates Encounter, Shop, or Event for each point aside from preset nodes
        foreach (Vector3 Point in Points)
        {
            int random = UnityEngine.Random.Range(1, 4);
            if (random == 1)
            {
                CreateEncounter(Point);
            }
            else if (random == 2)
            {
                CreateShop(Point);
            }
            else if (random == 3)
            {
                CreateEvent(Point);
            }
        }
        MoveBossAndExit();
        CreateStairs(new Vector3(exit.transform.position.x - 50, exit.transform.position.y), horStairs);

        // Creates path and stairs
        CreatePath(path1);
        CreatePath(path2);
        CreatePath(path3);
        InstantiateStairs();

        // removes any unused assets generated
        GameObject[] unusedObjects = GameObject.FindGameObjectsWithTag("NotUsed");
        foreach (GameObject node in unusedObjects)
        {
            Destroy(node);
        }
    }

    void MoveBossAndExit()
    {
        exit.transform.position = new Vector3(OrientingPosition.x + 100 * (numberOfRooms + 1), OrientingPosition.y + 173.2f);
        Boss.transform.position = new Vector3(OrientingPosition.x + 100 * (numberOfRooms), OrientingPosition.y + 173.2f);
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
        else if (point.y == OrientingPosition.y + 86.6f)
        {
            row2.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
        else if (point.y == OrientingPosition.y + 173.2f)
        {
            row3.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
    }
    // Sorts nodes into rows to draw lines
    void CreatePath(List<GameObject> pathNodes)
    {
        bool repeat = true;
        while (repeat == true)
        {
            pathNodes.Clear();
            pathNodes.Add(start);
            int count = 1;
            for (int i = 1; i < numberOfRooms + 2; i++)
            {
                int random = UnityEngine.Random.Range(0, 2);
                if (count == 1)
                {
                    if (random == 0 && i < 4)
                    {
                        pathNodes.Add(row1[i]);
                        row1[i].tag = KeepTag;
                    }
                    else
                    {
                        pathNodes.Add(row2[i - 1]);
                        row2[i - 1].tag = KeepTag;
                        count += 1;
                    }
                }
                else if (count == 2)
                {
                    if (random == 0 && i < 5)
                    {
                        pathNodes.Add(row2[i - 1]);
                        row2[i - 1].tag = KeepTag;
                    }
                    else
                    {
                        pathNodes.Add(row3[i - 2]);
                        row3[i - 2].tag = KeepTag;
                        count += 1;
                    }
                }
                else
                {
                    pathNodes.Add(row3[i - 2]);
                    row3[i - 2].tag = KeepTag;
                }
            }
            if (paths.Count == 0)
            {
                paths.Add(pathNodes);
                repeat = false;
                for (int i = 0; i < pathNodes.Count - 1; i++)
                {
                    pathNodes[i].GetComponent<MapNode>().nextNodes.Add(pathNodes[i + 1].GetComponent<MapNode>());
                    pathNodes[i + 1].GetComponent<MapNode>().prevNodes.Add(pathNodes[i].GetComponent<MapNode>());
                }
            }
            else
            {
                if (!CheckDuplicatePath(pathNodes))
                {
                    paths.Add(pathNodes);
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
        ListStairPoints(StairsPoints, path1);
        ListStairPoints(StairsPoints, path2);
        ListStairPoints(StairsPoints, path3);
        for (int i = 0; i < StairsPoints.Count; i++)
        {
            CreateStairs(StairsPoints[i], stairDirections[i]);
        }
    }
    // Generates stairs

    bool CheckDuplicatePath(List<GameObject> path)
    {
        bool duplicate = false;
        for (int i = 0; i < paths.Count; i++)
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
}
