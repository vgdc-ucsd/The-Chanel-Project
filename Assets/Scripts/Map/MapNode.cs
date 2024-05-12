using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class MapNode : MonoBehaviour,
IPointerClickHandler,
IPointerEnterHandler,
IPointerExitHandler
{
    public bool visited;
    public bool locked = true;
    public MapNodeType mapNodeType;
    public List<MapNode> nextNodes;
    public List<MapNode> prevNodes;

    private bool initialized;
    public int row = 1;
    private MapGrid mapGrid;

    private void Start()
    {
        // Set locked to true initially
        locked = true;
        mapGrid = FindObjectOfType<MapGrid>();
    }

    private void Update()
    {
        // Initialize here because it is called after Start
        if (!initialized)
        {
            visited = gameObject == mapGrid.row1[0];
            if (visited == true)
            {
                foreach (var node in nextNodes)
                {
                    node.locked = false;
                }
            }
            initialized = true;
        }
    }

    /// <summary>
    /// This method is called when this map node is clicked
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked || visited) return;
        visited = true;
        locked = true;

        // TODO: Show that the node has been visited
        // This can be done by doing a either a sprite swap or instantiate a 'X' on it

        // Lock all other nodes
        FindObjectOfType<MapGrid>().LockSiblingNodes();

        // Unlock all of its nextNodes
        foreach (var node in nextNodes)
        {
            node.locked = false;
        }

        // TODO: Load new scene after click
        int mapNodeTypeIdx = -1;
        if (mapNodeType == MapNodeType.Encounter)
        {
            mapNodeTypeIdx = 2;
        }
        else if (mapNodeType == MapNodeType.Shop)
        {
            mapNodeTypeIdx = 3;
        }
        else if (mapNodeType == MapNodeType.Event)
        {
            mapNodeTypeIdx = 4;
        }
        else if (mapNodeType == MapNodeType.Boss)
        {
            mapNodeTypeIdx = 5;
        }
        else if (mapNodeType == MapNodeType.StartOrExit)
        {
            mapNodeTypeIdx = -1;
        }

        if (mapNodeTypeIdx != -1)
        {
            FindObjectOfType<ChangeScene>().MoveToScene(mapNodeTypeIdx);
        }
    }

    /// <summary>
    /// This method is called when the mouse enters this node. It is used to show
    /// that the mouse is 'hovering' over the node.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (locked || visited) return;

        // TODO: Show that the node is selected / being hovered on
        // This can be done either by sprite swap or change color maybe?
    }

    /// <summary>
    /// This method is called when the mouse exits this node. It is used to show
    /// that the mouse is no longer 'hovering' over the node.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (locked || visited) return;

        // TODO: Show that the node is no longer selected / being hovered on
        // This can be done either by sprite swap or change color maybe?
    }
}

// This is an enum type to show all the possible node types
public enum MapNodeType
{
    Encounter,
    Event,
    Shop,
    Boss,
    StartOrExit
}