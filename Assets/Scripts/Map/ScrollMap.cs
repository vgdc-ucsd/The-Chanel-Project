using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class ScrollMap : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] float sidePadding;
    [SerializeField] RectTransform nodes;

    private List<RectTransform> children = new();
    private MapGenerator mapGridRef;
    void Start()
    {
        Invoke("Init", 0.1f);
        mapGridRef = FindObjectOfType<MapGenerator>();
    }

    public void Init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // NODES MOVE LEFT, MAP SCROLLS TO THE RIGHT
        {
            float endNodeXPos = mapGridRef.exitInstace.GetComponent<RectTransform>().localPosition.x - (10 * sensitivity) + nodes.localPosition.x;
            if (endNodeXPos > (GetComponent<RectTransform>().sizeDelta.x / 2f) - sidePadding)
            {
                foreach (var child in children)
                {
                    child.localPosition = new Vector3(child.localPosition.x - (10 * sensitivity), child.localPosition.y, child.localPosition.z);
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0) // NODES MOVE RIGHT, MAP SCROLLS LEFT
        {
            float startNodeXPos = mapGridRef.startInstace.GetComponent<RectTransform>().localPosition.x + (10 * sensitivity) + nodes.localPosition.x;
            if (startNodeXPos < -(GetComponent<RectTransform>().sizeDelta.x / 2f) + sidePadding)
            {
                foreach (var child in children)
                {
                    child.localPosition = new Vector3(child.localPosition.x + (10 * sensitivity), child.localPosition.y, child.localPosition.z);
                }
            }
        }
    }
}
