using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<List<MapNode>> mapNodes = new();

    // TESTING PURPOSES FIELDS
    [SerializeField] List<MapNode> layer1;
    [SerializeField] List<MapNode> layer2;
    [SerializeField] List<MapNode> layer3;
    [SerializeField] List<MapNode> layer4;
    // END OF TESTING PURPOSES FIELDS

    private void Awake()
    {
        mapNodes.Add(layer1);
        mapNodes.Add(layer2);
        mapNodes.Add(layer3);
        mapNodes.Add(layer4);
    }

    /// <summary>
    /// This method is used to lock all other nodes on the same layer as the 
    /// selected node. This ensures the player is not able to travel to another
    /// node on that layer.
    /// </summary>
    /// <param name="selectedNode"></param>
    public void LockSiblingNodes(MapNode selectedNode)
    {
        foreach (var node in mapNodes[selectedNode.layer])
        {
            node.locked = true;
        }
    }
}
