using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollMap : MonoBehaviour
{
    private List<RectTransform> children = new();
    void Start()
    {
        Invoke("Init", 0.1f);
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
            foreach (var child in children)
            {
                child.localPosition = new Vector3(child.localPosition.x - 100, child.localPosition.y, child.localPosition.z);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            foreach (var child in children)
            {
                child.localPosition = new Vector3(child.localPosition.x + 100, child.localPosition.y, child.localPosition.z);
            }
        }
    }
}
