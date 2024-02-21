using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenerateMap : MonoBehaviour
{
    public GameObject Encounter;
    public GameObject start;
    public LineRenderer line;
    public Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        //sets start location based on starting node/area to generate the map
        startPos = new Vector3(start.transform.position.x, start.transform.position.y);
        transform.rotation = Quaternion.identity;
        
        Instantiate(Encounter, new Vector3(startPos.x + 100, startPos.y),transform.rotation, Encounter.transform.parent);
        line.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0,startPos);
        line.SetPosition(1,new Vector3(startPos.x+100, startPos.y));
    }
}
