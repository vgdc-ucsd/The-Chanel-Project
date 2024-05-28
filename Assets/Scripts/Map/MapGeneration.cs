using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;

public class MapGeneration : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapCol;
    public Vector3 mapOffset;

    [Header("References")]
    public MapNode mapNodeTemplate;
    public Transform mapContainer;
    public LineRenderer pathTemplate;
    public ScrollMap scrollMap;
    public MapCharacterController character;

    [HideInInspector] public GameObject startInstance;
    [HideInInspector] public GameObject exitInstance;
    [HideInInspector] public MapNode lastVisitedNode;

    private List<MapNode[]> mapNodes;
    public List<GameObject> allNodes = new();
    private const int mapRow = 3;

    private const float branchChance = 0.6f;


    const int MIN_SHOP_DIST = 5;
    const int MAX_SHOP_DIST = 8;

    const int MIN_BRANCH_COUNT = 6;
    const int MAX_BRANCH_COUNT = 7;
    const int MIN_NODE_COUNT = 18;
    const int MAX_NODE_COUNT = 21;

    const float SHOP_CHANCE = 0.3f;

    void Start()
    {
        // Debug.Log($"Generated Map in {iter} iterations");

        PersistentData.Instance.VsState = VsScreenState.VS;

        if (PersistentData.Instance.mapInfo.nodePoints.Count != 0)
        {
            LoadMap(PersistentData.Instance.mapInfo);
        }
        else
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
        }
    }

    public void GenerateMap()
    {
        allNodes = new();
        mapNodes = new List<MapNode[]>();
        for (int i = 0; i <= mapCol; i++)
        {
            mapNodes.Add(new MapNode[mapRow]);
        }

        // Choose Starting Point
        int startNodeRow = 1;
        MapNode startNode = CreateMapNode(0, startNodeRow);
        startNode.mapNodeType = MapNodeType.Start;
        startNode.visited = true;

        // Setup First 2 Encounters from startNode
        MapNode node1 = CreateMapNode(1, 1);
        startNode.nextNodes.Add(node1);
        node1.prevNodes.Add(startNode);
        startNode.connections.Add(new Connection(startNode.point, node1.point));
        MapNode node2 = CreateMapNode(1, 0);
        startNode.nextNodes.Add(node2);
        node2.prevNodes.Add(startNode);
        startNode.connections.Add(new Connection(startNode.point, node2.point));

        // Choose endNode Point
        int exitNodeRow = Random.Range(0, mapRow);
        MapNode exitNode = CreateMapNode(mapCol, exitNodeRow);
        exitNode.mapNodeType = MapNodeType.Boss;

        // Create Paths
        for (int i = 1; i < mapCol - 1; i++)
        {
            foreach (MapNode node in mapNodes[i])
            {
                if (node != null)
                    CreatePath(node, exitNode);
            }
        }

        foreach (var node in mapNodes[mapCol - 1])
        {
            if (node != null)
            {
                node.nextNodes.Add(exitNode);
                exitNode.prevNodes.Add(node);
                node.connections.Add(new Connection(node.point, exitNode.point));
            }
        }

        // Add positional offset
        InitializeAllNodesList();
        foreach (var node in allNodes)
        {
            node.GetComponent<RectTransform>().localPosition += mapOffset;
        }


        // Set startInstace, exitInstance, lastVisitedNode
        lastVisitedNode = startNode;
        startInstance = startNode.gameObject;
        exitInstance = exitNode.gameObject;

        PlaceEvents();
        PlaceShops();

        DrawNodes();
        DrawPaths();

        Vector2 charPos = startNode.GetComponent<RectTransform>().localPosition;
        character.SetPosition(charPos);

        SaveMap();
    }

    public void SaveMap()
    {
        List<MapNodeType> nodeTypes = new();
        List<Point> nodePoints = new();
        List<ConnectionsList> nodeConnections = new();
        List<bool> nodeVisited = new();
        for (int i = 0; i < allNodes.Count; i++)
        {
            MapNode curNode = allNodes[i].GetComponent<MapNode>();
            ConnectionsList connectionsList = new();
            if (curNode != null)
            {
                nodeTypes.Add(curNode.mapNodeType);
                nodePoints.Add(curNode.point);
                connectionsList.connections = curNode.connections;
                nodeConnections.Add(connectionsList);
                nodeVisited.Add(curNode.visited);
            }
        }

        PersistentData.Instance.mapInfo.nodeTypes = nodeTypes;
        PersistentData.Instance.mapInfo.nodePoints = nodePoints;
        PersistentData.Instance.mapInfo.nodeConnections = nodeConnections;
        PersistentData.Instance.mapInfo.nodeVisited = nodeVisited;
        PersistentData.Instance.mapInfo.lastVisitedNode = lastVisitedNode.point;
    }

    public void LoadMap(MapInfo mapInfo)
    {
        // for (int i = 0; i < mapInfo.nodePoints.Count; i++)
        // {
        //     Point point = mapInfo.nodePoints[i];
        //     newNode.GetComponent<RectTransform>().localPosition += new Vector3(newNode.point.x * 120, newNode.point.y * 100, 0);
        //     if (newNode.point.x % 2 == 1)
        //     {
        //         newNode.GetComponent<RectTransform>().localPosition += new Vector3(0, 50);
        //     }

        //     Vector2 pos = new Vector2(OrientingPosition.x + (point.x * distanceBetweenNodes) + (point.y * XOffsetDistanceBetweenRows), OrientingPosition.y + (point.y * heightBetweenRows));
        //     GameObject mapTypePrefab = null;
        //     switch (mapInfo.nodeTypes[i])
        //     {
        //         case MapNodeType.Start:
        //             mapTypePrefab = start;
        //             break;
        //         case MapNodeType.Encounter:
        //             mapTypePrefab = Encounter;
        //             break;
        //         case MapNodeType.Event:
        //             mapTypePrefab = Event;
        //             break;
        //         case MapNodeType.Shop:
        //             mapTypePrefab = Shop;
        //             break;
        //         case MapNodeType.Boss:
        //             mapTypePrefab = Boss;
        //             break;
        //         case MapNodeType.Exit:
        //             mapTypePrefab = exit;
        //             break;
        //     }

        //     if (mapTypePrefab == null)
        //     {
        //         Debug.Log("MapTypePrefab is null in LoadMap()");
        //     }

        //     GameObject instantiated = Instantiate(mapTypePrefab, pos, transform.rotation, contents.transform);
        //     MapNode instantiatedMapNode = instantiated.GetComponent<MapNode>();

        //     if (point.y == 0) row1.Add(instantiated);
        //     else if (point.y == 1) row2.Add(instantiated);
        //     else if (point.y == 2) row3.Add(instantiated);

        //     allNodes.Add(instantiated);

        //     if (mapTypePrefab == start) startInstace = instantiated;
        //     if (mapTypePrefab == exit) exitInstace = instantiated;

        //     instantiatedMapNode.point = point;
        //     instantiatedMapNode.connections = mapInfo.nodeConnections[i].connections;
        //     instantiatedMapNode.visited = mapInfo.nodeVisited[i];
        //     instantiatedMapNode.initialized = true;
        //     instantiated.tag = "UsedNodes";

        //     instantiated.GetComponent<RectTransform>().localPosition += offsetPosition;
        // }

        // the code block above checks node type, instantiates the correct map node at the correct position, adds to allNodes list, ensures all mapNode fields are set
        // it also sets startInstance and exitInstance

        // Initialize mapNodes list to be non-null
        mapNodes = new List<MapNode[]>();
        for (int i = 0; i <= mapCol; i++)
        {
            mapNodes.Add(new MapNode[mapRow]);
        }

        for (int i = 0; i < mapInfo.nodePoints.Count; i++)
        {
            Point point = mapInfo.nodePoints[i];
            mapNodes[point.x][point.y] = CreateMapNode(point);
            MapNode node = mapNodes[point.x][point.y];
            node.mapNodeType = mapInfo.nodeTypes[i];

            if (node.mapNodeType == MapNodeType.Start) { startInstance = node.gameObject; }
            if (node.mapNodeType == MapNodeType.Boss) { exitInstance = node.gameObject; }

            node.connections = mapInfo.nodeConnections[i].connections;
            node.visited = mapInfo.nodeVisited[i];
            node.initialized = true;

            allNodes.Add(node.gameObject);
        }

        foreach (var node in allNodes)
        {
            foreach (var connection in node.GetComponent<MapNode>().connections)
            {
                GameObject nextNode = allNodes.Find(x => x.GetComponent<MapNode>().point.x == connection.point2.x && x.GetComponent<MapNode>().point.y == connection.point2.y);
                if (!node.GetComponent<MapNode>().nextNodes.Contains(nextNode.GetComponent<MapNode>()))
                {
                    node.GetComponent<MapNode>().nextNodes.Add(nextNode.GetComponent<MapNode>());
                }

                if (!nextNode.GetComponent<MapNode>().prevNodes.Contains(node.GetComponent<MapNode>()))
                {
                    nextNode.GetComponent<MapNode>().prevNodes.Add(node.GetComponent<MapNode>());
                }
            }
        }

        lastVisitedNode = allNodes.Find(x => x.GetComponent<MapNode>().point.x == mapInfo.lastVisitedNode.x && x.GetComponent<MapNode>().point.y == mapInfo.lastVisitedNode.y).GetComponent<MapNode>();
        LockSiblingNodes(lastVisitedNode);

        foreach (GameObject obj in allNodes)
        {
            MapNode n = obj.GetComponent<MapNode>();
            if (!n.locked || n.visited)
            {
                n.GetComponent<Image>().color = Color.white;
            }
            else
            {
                n.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }

            // Add positional offset to all nodes
            obj.GetComponent<RectTransform>().localPosition += mapOffset;
        }

        DrawNodes();
        DrawPaths();

        scrollMap.Init();

        Vector2 charPos = lastVisitedNode.GetComponent<RectTransform>().localPosition;
        character.SetPosition(charPos);
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
            node.connections.Add(new Connection(node.point, node1.point));
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
                node.connections.Add(new Connection(node.point, node1.point));
                //CreateNodeAt(node1, pt);
            }
        }
    }

    void InitializeAllNodesList()
    {
        foreach (var col in mapNodes)
        {
            foreach (var node in col)
            {
                if (node != null) { allNodes.Add(node.gameObject); }
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

                    newNode.GetComponent<RectTransform>().localPosition += new Vector3(newNode.point.x * 104, newNode.point.y * 100, 0);
                    if (newNode.point.x % 2 == 1)
                    {
                        newNode.GetComponent<RectTransform>().localPosition += new Vector3(0, 50);
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

    private List<Point> CheckForExit(List<Point> possiblePoints, MapNode node, MapNode exitNode)
    {
        int horDist = Mathf.Abs(exitNode.point.x - 1 - node.point.x);
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

    public void LockSiblingNodes(MapNode lastVisitedNode)
    {
        foreach (var node in allNodes)
        {
            node.GetComponent<MapNode>().locked = true;
            node.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }

        foreach (var nextNode in lastVisitedNode.nextNodes)
        {
            nextNode.locked = false;
        }
    }

}
