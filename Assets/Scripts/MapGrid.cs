using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapGrid : MonoBehaviour
{
    public List<Vector3> Points;
    public List<GameObject> row1;
    public List<GameObject> row2;
    public List<GameObject> row3;
    public GameObject start;
    public GameObject exit;
    public GameObject Encounter;
    public GameObject Shop;
    public GameObject Event;
    public GameObject Boss;
    public Vector3 OrientingPosition;
    public int numberOfRooms;

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
        for(int i = row1.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + 100 * i, OrientingPosition.y);
        }
        for(int i = row2.Count; i < numberOfRooms; i++)
        {
            AddPoints(OrientingPosition.x + 50 + 100 * i, OrientingPosition.y + 86.6f);
        }
        for(int i = row3.Count; i < numberOfRooms; i++)
        {
            if(i != numberOfRooms - 1)
            {
                AddPoints(OrientingPosition.x + 100 * (i + 1), OrientingPosition.y + 173.2f);
            }
            else
            {
                CreateBoss(new Vector3(OrientingPosition.x + 100 * (i + 1), OrientingPosition.y + 173.2f));
                CreateExit(new Vector3(OrientingPosition.x + 100 * (i + 2), OrientingPosition.y + 173.2f));
            }
        }
        transform.rotation = Quaternion.identity;

        // Generates Encounter, Shop, or Event for each point aside from preset nodes
        foreach(Vector3 Point in Points)
        {
            int random = Random.Range(1, 4);
            if(random == 1)
            {
                CreateEncounter(Point);
            }
            else if(random == 2)
            {
                CreateShop(Point);
            }
            else if(random == 3)
            {
                CreateEvent(Point);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
    void CreateBoss(Vector3 point)
    {
        SortNodeRow(point, Boss);
    }
    // Instantiates Boss at point
    void CreateExit(Vector3 point)
    {
        SortNodeRow(point, exit);
    }
    // Instantiates Exit at point
    void SortNodeRow(Vector3 point, GameObject type)
    {
        if(point.y == OrientingPosition.y)
        {
            row1.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
        else if(point.y == OrientingPosition.y + 86.6f)
        {
            row2.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
        else if(point.y == OrientingPosition.y + 173.2f)
        {
            row3.Add(Instantiate(type, point, transform.rotation, type.transform.parent));
        }
    }
    // Sorts nodes into rows to draw lines
}



