using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    public Animator animations;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Awake() {
        animations.Play("stand");
    }

    void Update()
    {
        if (isMoving)
        {
            animations.Play("walk");
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime * (1/walkTime));
            float xPos = Mathf.Lerp(initialPos.x, endPos.x, t);
            float yPos = Mathf.Lerp(initialPos.y, endPos.y, t);
            rt.localPosition = new(xPos, yPos);

            if (t >= 1)
            {
                animations.Play("stand");
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
