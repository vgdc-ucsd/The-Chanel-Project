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
    public Canvas MainCanvas;
    public Camera OverlayCamera;

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
        this.endPos = endPos*(300f/410f);
        //this.endPos = new Vector2(endPos.x + OverlayCamera.pixelWidth/2f, endPos.y + OverlayCamera.pixelHeight/2f);
        callerNode = node;
    }

    public void SetPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().localPosition = pos*(300f/410f);
    }
}
