using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    private const float branchChance = 0.6f;
    public MapNode mapNodeTemplate;
    public Transform mapContainer;
    public LineRenderer pathTemplate;

    const int MIN_SHOP_DIST = 5;
    const int MAX_SHOP_DIST = 8;

    const int MIN_BRANCH_COUNT = 6;
    const int MAX_BRANCH_COUNT = 7;
    const int MIN_NODE_COUNT = 18;
    const int MAX_NODE_COUNT = 21;

    const float SHOP_CHANCE = 0.3f;

    void Start()
    {
        int iter = 0;
        bool mapAccepted = false;
        while (!mapAccepted)
        {
            int nodeCount = 0;
            int branchCount = 0;
            GenerateMap();
            foreach (MapNode[] arr in mapNodes)
            {
                foreach (MapNode node in arr)
                {
                    if (node != null)
                    {
                        nodeCount++;
                        if (node.nextNodes.Count == 2) branchCount++;
                    }
                }
            }
            mapAccepted = (branchCount >= MIN_BRANCH_COUNT && branchCount <= MAX_BRANCH_COUNT) &&
                          (nodeCount >= MIN_NODE_COUNT && nodeCount <= MAX_NODE_COUNT);
            if (iter > 100) mapAccepted = true;
            if (!mapAccepted)
            {
                foreach (MapNode[] arr in mapNodes)
                {
                    foreach (MapNode node in arr)
                    {
                        if (node != null)
                        {
                            Destroy(node.gameObject);
                        }
                    }
                }
            }
            iter++;
        }

        PlaceEvents();
        PlaceShops();

        DrawNodes();
        DrawPaths();

        Debug.Log($"Generated Map in {iter} iterations");

        PersistentData.Instance.VsState = VsScreenState.VS;

        if (PersistentData.Instance.mapInfo.nodePoints.Count != 0)
        {
            // LoadMap(PersistentData.Instance.mapInfo);
        }
        else
        {
            //GenerateMap();
        }
    }


    public void GenerateMap()
    {
        mapNodes = new List<MapNode[]>();
        for (int i = 0; i < mapCol; i++)
        {
            mapNodes.Add(new MapNode[mapRow]);
        }
        // Set Starting Point
        int startNodeRow = 1;
        MapNode startNode = CreateMapNode(0, startNodeRow);
        startNode.mapNodeType = MapNodeType.Start;

        MapNode node1 = CreateMapNode(1, 1);
        startNode.nextNodes.Add(node1);
        node1.prevNodes.Add(startNode);
        MapNode node2 = CreateMapNode(1, 0);
        startNode.nextNodes.Add(node2);
        node2.prevNodes.Add(startNode);

        // Choose Exit Point
        int exitNodeRow = Random.Range(0, mapRow);
        MapNode exitNode = CreateMapNode(mapCol - 1, exitNodeRow);
        exitNode.mapNodeType = MapNodeType.Exit;

        for (int i = 1; i < mapCol - 1; i++)
        {
            // Create Paths
            foreach (MapNode node in mapNodes[i])
            {
                if (node != null)
                    CreatePath(node, exitNode);
            }
        }



        // Instantiate nodes at correct points
        // Instantiate the connections/lines between nodes

        // Add positional offset?

        // SaveMap();
        // Set startInstace, exitInstance, lastVisitedNode
    }

    private void PlaceEvents()
    {
        // cols 2-3, 5-6: force 3 events in 2 columns
        PlaceEventWall(2, 3);
        PlaceEventWall(5, 6);

    }

    void PlaceEventWall(int col1, int col2)
    {
        List<MapNode> eventWall = new List<MapNode>();
        eventWall.AddRange(mapNodes[col1]);
        eventWall.AddRange(mapNodes[col2]);
        for (int i = eventWall.Count - 1; i >= 0; i--)
        {
            if (eventWall[i] == null) eventWall.Remove(eventWall[i]);
        }
        int eventCount = Mathf.Min(3, eventWall.Count);

        for (int i = 0; i < eventCount; i++)
        {
            int idx = Random.Range(0, eventWall.Count);
            MapNode eventNode = eventWall[idx];
            eventWall.RemoveAt(idx);
            eventNode.mapNodeType = MapNodeType.Event;
        }
    }

    private void PlaceShops()
    {
        int i = 0;
        foreach (MapNode[] arr in mapNodes)
        {
            foreach (MapNode node in arr)
            {
                if (node != null)
                {
                    i++;
                    if (node != null && node.mapNodeType != MapNodeType.Event)
                    {
                        if (i >= MAX_SHOP_DIST)
                        {
                            node.mapNodeType = MapNodeType.Shop;
                            i = 0;
                        }
                        else if (i >= MIN_SHOP_DIST)
                        {
                            if (Random.value < SHOP_CHANCE)
                            {
                                node.mapNodeType = MapNodeType.Shop;
                                i = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    private void CreatePath(MapNode node, MapNode exitNode)
    {
        List<Point> possiblePoints = new();
        possiblePoints.Add(new Point(node.point.x + 1, node.point.y));
        if (node.point.x % 2 == 0 && node.point.y != 0)
        {
            possiblePoints.Add(new Point(node.point.x + 1, node.point.y - 1));
        }
        else if (node.point.x % 2 == 1 && node.point.y != mapRow - 1)
        {
            possiblePoints.Add(new Point(node.point.x + 1, node.point.y + 1));
        }

        possiblePoints = CheckForExit(possiblePoints, node, exitNode);

        if (Random.value > branchChance)
        {
            // 1 next node
            Point newNodePos = possiblePoints[Random.Range(0, possiblePoints.Count)];
            MapNode node1 = CreateMapNode(newNodePos);
            node.nextNodes.Add(node1);
            node1.prevNodes.Add(node);
            //CreateNodeAt(node1, newNodePos);
        }
        else
        {
            // 2 next nodes
            foreach (Point pt in possiblePoints)
            {
                MapNode node1 = CreateMapNode(pt);
                node.nextNodes.Add(node1);
                node1.prevNodes.Add(node);
                //CreateNodeAt(node1, pt);
            }
        }
    }

    public MapNode CreateMapNode(int x, int y)
    {
        if (mapNodes[x][y] != null)
        {
            return mapNodes[x][y];
        }
        MapNode newNode = Instantiate(mapNodeTemplate);
        newNode.point = new Point(x, y);
        mapNodes[x][y] = newNode;

        return newNode;
    }

    public MapNode CreateMapNode(Point point)
    {
        return CreateMapNode(point.x, point.y);
    }

    void DrawNodes()
    {
        foreach (MapNode[] nodeArr in mapNodes)
        {
            foreach (MapNode newNode in nodeArr)
            {
                if (newNode != null)
                {
                    newNode.transform.SetParent(mapContainer);

                    newNode.transform.position = new Vector3(newNode.point.x * 120, newNode.point.y * 100);
                    if (newNode.point.x % 2 == 1)
                    {
                        newNode.transform.position += new Vector3(0, 50);
                    }
                    newNode.DrawMapNodeType();
                }

            }

        }
    }

    void DrawPaths()
    {
        foreach (MapNode[] arr in mapNodes)
        {
            foreach (MapNode node in arr)
            {
                if (node != null)
                    foreach (MapNode nextNode in node.nextNodes)
                    {
                        LineRenderer lr = Instantiate(pathTemplate);
                        lr.transform.SetParent(node.transform);
                        lr.SetPosition(0, node.transform.position);
                        lr.SetPosition(1, nextNode.transform.position);
                    }
            }
        }
    }

    /*
    void InstantiateStairs()
    {
        foreach (MapNode node in mapNodes)
        {
            foreach (var nextNode in node.nextNodes)
            {
                float yDist = nextNode.point.y == node.point.y ? 0f : heightBetweenRows;
                float zAngle = Mathf.Atan2(yDist, XOffsetDistanceBetweenRows) * Mathf.Rad2Deg;
                Vector3 angle = new Vector3(0, 0, zAngle);
                GameObject stairObj = Instantiate(stair, node.transform.position, Quaternion.Euler(angle), node.transform);
                if (nextNode.point.y == node.point.y)
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
    */

    private List<Point> CheckForExit(List<Point> possiblePoints, MapNode node, MapNode exitNode)
    {
        int horDist = Mathf.Abs(exitNode.point.x - node.point.x);
        int vertDist = exitNode.point.y - node.point.y;


        if ((vertDist + 1) >= horDist)
        {
            for (int i = 0; i < possiblePoints.Count; i++)
            {
                if (node.point.x % 2 == 0)
                {
                    if (possiblePoints[i].y == node.point.y - 1) { possiblePoints.Remove(possiblePoints[i]); }
                }
                else if (node.point.x % 2 == 1)
                {
                    if (possiblePoints[i].y == node.point.y) { possiblePoints.Remove(possiblePoints[i]); }
                }
            }
        }
        else if ((vertDist - 2) <= -horDist)
        {
            for (int i = 0; i < possiblePoints.Count; i++)
            {
                if (node.point.x % 2 == 0)
                {
                    if (possiblePoints[i].y == node.point.y) { possiblePoints.Remove(possiblePoints[i]); }
                }
                else if (node.point.x % 2 == 1)
                {
                    if (possiblePoints[i].y == node.point.y + 1) { possiblePoints.Remove(possiblePoints[i]); }
                }
            }
        }

        return possiblePoints;
    }
}
