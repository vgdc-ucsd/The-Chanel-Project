using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCharacterController : MonoBehaviour
{
    public float walkTime;
    public bool isMoving;
    private RectTransform rt;
    float elapsedTime;
    float startTime;
    Vector2 initialPos;
    Vector2 endPos;
    MapNode callerNode;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isMoving)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime);
            float xPos = Mathf.Lerp(initialPos.x, endPos.x, t);
            float yPos = Mathf.Lerp(initialPos.y, endPos.y, t);
            rt.localPosition = new(xPos, yPos);

            if (elapsedTime >= walkTime)
            {
                isMoving = false;
                callerNode.LoadSceneAfterDelay();
            }
        }
    }

    public void Move(Vector2 endPos, MapNode node)
    {
        isMoving = true;
        startTime = Time.time;
        elapsedTime = 0f;
        initialPos = rt.localPosition;
        this.endPos = endPos;
        callerNode = node;
    }

    public void SetPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().localPosition = pos;
    }
}
