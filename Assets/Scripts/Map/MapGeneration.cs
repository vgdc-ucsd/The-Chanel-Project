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

    private const float branchChance = 0.4f;
    public MapNode mapNodeTemplate;
    public Transform mapContainer;
    public LineRenderer pathTemplate;

    void Start()
    {
        
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
            mapAccepted = (branchCount >= 6 && branchCount <= 8) && 
                          (nodeCount >= 19 && nodeCount <= 23);
            if (!mapAccepted)
            {
                foreach (MapNode[] arr in mapNodes)
                {
                    foreach (MapNode node in arr)
                    {
                        if (node != null)
                        {
                            Destroy(node);
                        }
                    }
                }
            }
        }
        DrawNodes();
        DrawPaths();

        // 6 - 8 branch
        // 19 - 23 total


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
        // Choose Starting Point
        int startNodeRow = 1;
        MapNode startNode = CreateMapNode(0, startNodeRow);
        startNode.mapNodeType = MapNodeType.Start;

        MapNode node1 = CreateMapNode(1,1);
        startNode.nextNodes.Add(node1);
        node1.prevNodes.Add(startNode);
        MapNode node2 = CreateMapNode(1,0);
        startNode.nextNodes.Add(node2);
        node2.prevNodes.Add(startNode);

        for (int i = 1; i < mapCol - 1; i++)
        {
            // Create Paths
            foreach (MapNode node in mapNodes[i])
            {
                if (node != null)
                    CreatePath(node);
            }
            
        }
        foreach (MapNode[] arr in mapNodes)
        {
            Debug.Log(arr.ToSeparatedString(","));

        }

        

        // Instantiate nodes at correct points
        // Instantiate the connections/lines between nodes

        // Add positional offset?

        // SaveMap();
        // Set startInstace, exitInstance, lastVisitedNode
    }


    private void CreatePath(MapNode node)
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
}
