using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

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
    public List<Connection> connections = new();

    [HideInInspector] public bool initialized;
    // public int row = 1;
    public Point point;
    private MapGeneration mapGenerator;
    private MapCharacterController character;

    public Image image;
    public Sprite encounterIcon, eventIcon, shopIcon, bossIcon;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGeneration>();
        character = mapGenerator.character;
    }

    private void Update()
    {
        // Initialize here because it is called after Start
        if (!initialized)
        {
            visited = gameObject == mapGenerator.startInstance;
            if (visited == true)
            {
                foreach (var node in nextNodes)
                {
                    node.locked = false;
                    node.GetComponent<Image>().color = Color.white;
                }

            }
            initialized = true;
        }

        if (!locked && !character.isMoving) StartCoroutine(ManaFlicker());
    }

    public void DrawMapNodeType()
    {
        switch (mapNodeType)
        {
            case MapNodeType.Encounter:
                image.sprite = encounterIcon;
                break;
            case MapNodeType.Event:
                image.sprite = eventIcon;
                break;
            case MapNodeType.Shop:
                image.sprite = shopIcon;
                break;
            case MapNodeType.Boss:
                image.sprite = bossIcon;
                break;
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

        // Lock all other nodes
        mapGenerator.LockSiblingNodes(this);
        mapGenerator.lastVisitedNode = this;

        // Character script moves and then loads new scene
        Vector2 endPos = GetComponent<RectTransform>().localPosition + transform.parent.GetComponent<RectTransform>().localPosition;
        character.Move(endPos, this);
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

    public void LoadSceneAfterDelay()
    {
        int mapNodeTypeIdx = -1;
        if (mapNodeType == MapNodeType.Encounter)
        {
            // if (MenuScript.Instance != null)
            // {
            //     //int randomIndex = UnityEngine.Random.Range(0, EncounterManager.Instance.Encounters.Count);
            //     PersistentData.Instance.CurrentEncounter = EncounterManager.Instance.Encounters[randomIndex];
            // }
            PersistentData.Instance.SetEncounterStats();

            mapNodeTypeIdx = MenuScript.VERSUS_INDEX;
        }
        else if (mapNodeType == MapNodeType.Shop)
        {
            mapNodeTypeIdx = MenuScript.SHOP_INDEX;
        }
        else if (mapNodeType == MapNodeType.Event)
        {
            mapNodeTypeIdx = MenuScript.EVENT_INDEX;
        }
        else if (mapNodeType == MapNodeType.Boss)
        {
            PersistentData.Instance.CurrentEncounter = PersistentData.Instance.bossEncounters[1];

            mapNodeTypeIdx = MenuScript.VERSUS_INDEX;
        }
        else if (mapNodeType == MapNodeType.Exit)
        {
            mapNodeTypeIdx = MenuScript.TITLE_INDEX;
        }
        else if (mapNodeType == MapNodeType.Start)
        {
            mapNodeTypeIdx = -1;
        }

        if (mapNodeTypeIdx != -1)
        {
            mapGenerator.SaveMap();
            MenuScript.Instance.LoadScene(mapNodeTypeIdx);
        }
    }

    private IEnumerator ManaFlicker()
    {
        float startTime = Time.time;
        float duration = 1.0f;
        bool direction = true;
        Color white = Color.white;
        Color flicker = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Color col = Color.white;
        Vector2 initScale = GetComponent<RectTransform>().sizeDelta;
        Vector2 flickerScale = initScale * 1.1f;
        Vector2 scale = initScale;

        while (true)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;

            if (direction)
            {
                col = Interpolation.Interpolate(white, flicker, t, InterpolationMode.Linear);
                scale = Vector2.Lerp(flickerScale, initScale, t);

                if (elapsedTime > duration)
                {
                    direction = false;
                    startTime = Time.time;
                }
            }
            else
            {
                col = Interpolation.Interpolate(flicker, white, t, InterpolationMode.Linear);
                scale = Vector2.Lerp(initScale, flickerScale, t);

                if (elapsedTime > duration)
                {
                    direction = true;
                    startTime = Time.time;
                }
            }

            GetComponent<Image>().color = col;
            GetComponent<RectTransform>().sizeDelta = scale;

            yield return null;
        }
    }
}

// This is an enum type to show all the possible node types
public enum MapNodeType
{
    Encounter, Event,
    Shop, Boss,
    Start, Exit
}

[Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}