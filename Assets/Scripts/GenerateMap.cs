using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;


public class GenerateMap : MonoBehaviour
{
    public GameObject Encounter;
    public GameObject Shop;
    public GameObject Event;
    public GameObject start;
    public Vector3 Pos;
    public List<Vector3> positions = new();
    public List<Vector3> newPositions = new();
    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3> { new(start.transform.position.x, start.transform.position.y) };
        transform.rotation = Quaternion.identity;
        GenerateNodes(positions);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateNodes(List<Vector3> currentList)
    {
        for(int h = 0; h < 4; h++)
        {
            //creates new nodes/events
            for (int i = 0; i < currentList.Count; i++)
            {
                Pos = currentList[i];
                //nodes can only be created on upper two sides and right side of hexagon; (not sure the limit or direction of map yet)
                for (int j = 0; j < 3; j++)
                {
                    int eventType;
                    int directionNum;
                    GameObject clone = null;
                    GameObject nodeType;
                    List<Vector3> directions = new() { new(Pos.x + 100, Pos.y), new(Pos.x + 50, (float)(Pos.y + 86.6)), new(Pos.x - 50, (float)(Pos.y + 86.6)) };
                    newPositions = new();
                    //ensures at least one path from a node exists
                    if (newPositions.Count == 0)
                    {
                        eventType = Random.Range(1, 90);
                    }
                    else
                    {
                        eventType = Random.Range(1, 100);
                    }
                    //50% chance to generate an encounter(enemy event)
                    if (eventType <= 50)
                    {
                        nodeType = Encounter;
                    }
                    //30% chance to generate an event
                    else if (eventType <= 80)
                    {
                        nodeType = Event;
                    }
                    //19% chance to generate a shop
                    else if (eventType <= 99)
                    {
                        nodeType = Shop;
                    }
                    else
                    {
                        nodeType = null;
                    }
                    directionNum = Random.Range(0, 2);
                    if (directionNum == 0)
                    {
                        //creates encounter node to the east
                        CreateNode(nodeType, directions[directionNum], clone);
                    }
                    else if (directionNum == 1)
                    {
                        //creates encounter node to the northeast
                        CreateNode(nodeType, directions[directionNum], clone);
                    }
                    else if (directionNum == 2)
                    {
                        //creates encounter node to the northwest
                        CreateNode(nodeType, directions[directionNum], clone);
                    }
                    currentList = newPositions;
                }
            }
        }
    }

    void CreateNode(GameObject type, Vector3 pos, GameObject newObject)
    {
        Instantiate(type, pos, transform.rotation, Encounter.transform.parent);
        newObject = Instantiate(type, pos, transform.rotation, Encounter.transform.parent);
        newPositions.Add(new Vector3(newObject.transform.position.x, newObject.transform.position.y));
    }
}

