using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour,
IPointerClickHandler,
IPointerEnterHandler,
IPointerExitHandler
{
    // This is 0-indexed
    public int layer;
    /*
    I DON'T KNOW IF I NEED THIS YET
    public int y;
    */
    public bool visited;
    public bool locked;
    public MapNodeType mapNodeType;
    public List<MapNode> nextNodes;
    public List<MapNode> prevNodes;
    [SerializeField] LineRenderer lineRendererPrefab;
    [HideInInspector] public List<LineRenderer> lineRenderers;

    private void Awake()
    {
        InitializeNodes();
        ConnectPathToNextNodes();
    }

    /// <summary>
    /// This method sets up the node. This method ensures only first layer nodes
    /// are not locked and can be selected.
    /// </summary>
    private void InitializeNodes()
    {
        visited = false;
        if (layer != 0)
        {
            locked = true;
        }
    }

    /// <summary>
    /// This method draws lines to each of its nextNodes using Line Renderers.
    /// </summary>
    public void ConnectPathToNextNodes()
    {
        foreach (var node in nextNodes)
        {
            Vector3[] lrPositions = new Vector3[2] { transform.position, node.transform.position };
            LineRenderer lr = Instantiate(lineRendererPrefab, transform);
            lr.SetPositions(lrPositions);
            lr.gameObject.SetActive(true);
            lineRenderers.Add(lr);
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

        // TODO: this is a temporary color to show that the node has been visited
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        // Unlock all of its nextNodes
        foreach (var node in nextNodes)
        {
            node.locked = false;
        }

        // Changes color of line renderers pointing to this map node
        // The color change is temporary
        foreach (var node in prevNodes)
        {
            if (node.visited)
            {
                foreach (var lr in node.lineRenderers)
                {
                    if (lr.GetPosition(1) == transform.position)
                    {
                        lr.startColor = Color.white;
                        lr.endColor = Color.white;
                    }
                }
            }
        }

        // mapController reference may or may not be temporary
        MapController mapController = FindObjectOfType<MapController>();

        // This ensures nodes on the same layer cannot be accessed/are locked
        mapController.LockSiblingNodes(this);

        // TODO: Replace this line with loading a new scene in the future.
        Debug.Log("Load Next Scene: " + mapNodeType.ToString());
    }

    /// <summary>
    /// This method is called when the mouse enters this node. It is used to show
    /// that the mouse is 'hovering' over the node.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (locked || visited) return;
        // TODO: this is a temporary color change
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    /// <summary>
    /// This method is called when the mouse exits this node. It is used to show
    /// that the mouse is no longer 'hovering' over the node.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (locked || visited) return;
        // TODO: This is a temporary color change
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.1839623f, 0.1839623f, 0.1839623f, 1);
    }
}

// This is an enum type to show all the possible node types
public enum MapNodeType
{
    Enemy,
    Rest,
    Shop,
    Boss
}