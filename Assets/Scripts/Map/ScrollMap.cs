using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class ScrollMap : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] float sidePadding;

    private List<RectTransform> children = new();
    private MapGrid mapGridRef;
    void Start()
    {
        Invoke("Init", 0.1f);
        mapGridRef = FindObjectOfType<MapGrid>();
    }

    private void Init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            float endNodeXPos = mapGridRef.row3[mapGridRef.row3.Count - 1].GetComponent<RectTransform>().localPosition.x - (10 * sensitivity);
            if (endNodeXPos > (GetComponent<RectTransform>().sizeDelta.x / 2f) - sidePadding)
            {
                foreach (var child in children)
                {
                    child.localPosition = new Vector3(child.localPosition.x - (10 * sensitivity), child.localPosition.y, child.localPosition.z);
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            float startNodeXPos = mapGridRef.row1[0].GetComponent<RectTransform>().localPosition.x + (10 * sensitivity);
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
