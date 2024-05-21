// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Mathematics;
// using UnityEngine;
// using UnityEngine.UI;
// using Random = UnityEngine.Random;


// public class MapController : MonoBehaviour
// {
//     public List<List<MapNode>> mapNodes = new();
//     public MapConfig mapConfig;
//     public MapNode mapNodePrefab;
//     [SerializeField] GameObject nodesParent;

//     // TESTING PURPOSES FIELDS
//     // [SerializeField] List<MapNode> layer1;
//     // [SerializeField] List<MapNode> layer2;
//     // [SerializeField] List<MapNode> layer3;
//     // [SerializeField] List<MapNode> layer4;
//     // END OF TESTING PURPOSES FIELDS

//     private void Awake()
//     {
//         // mapNodes.Add(layer1);
//         // mapNodes.Add(layer2);
//         // mapNodes.Add(layer3);
//         // mapNodes.Add(layer4);
//         InitializeNodes();
//     }

//     /// <summary>
//     /// This method is used to lock all other nodes on the same layer as the 
//     /// selected node. This ensures the player is not able to travel to another
//     /// node on that layer.
//     /// </summary>
//     /// <param name="selectedNode"></param>
//     public void LockSiblingNodes(MapNode selectedNode)
//     {
//         foreach (var node in mapNodes[selectedNode.layer])
//         {
//             node.locked = true;
//         }
//     }

//     public void InitializeNodes()
//     {
//         for (int i = 0; i < mapConfig.layerOptions.Count; i++)
//         {
//             List<MapNode> nodesInLayer = new();
//             float offset = (mapConfig.layerOptions[i].numOfNodes - 1) / 2f;
//             for (int j = 0; j < mapConfig.layerOptions[i].numOfNodes; j++)
//             {
//                 MapNode node = Instantiate(mapNodePrefab, nodesParent.transform);
//                 node.layer = i;
//                 node.indexInLayer = j;
//                 node.mapNodeType = mapConfig.layerOptions[i].mapNodeType;

//                 node.transform.position = new Vector3((j - offset) * 1.5f, i * 1.5f, 0);

//                 nodesInLayer.Add(node);

//                 node.gameObject.SetActive(true);
//             }
//             mapNodes.Add(nodesInLayer);
//         }
//     }

//     private double DistanceBetweenNodes(MapNode node1, MapNode node2)
//     {
//         double distance = Math.Sqrt(Math.Pow(node1.transform.position.x - node2.transform.position.x, 2) +
//                             Math.Pow(node1.transform.position.y - node2.transform.position.y, 2));

//         return distance;

//     }
// }
