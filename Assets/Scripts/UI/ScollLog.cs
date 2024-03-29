using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScollLog : MonoBehaviour, IPointerDownHandler
{
    public float Closed;
    public float Open;
    public float Duration = 0.5f;
    
    private bool open;
    private QueueableAnimation anim;

    void Start() {
        open = false;
        transform.position = new Vector3(transform.position.x, Closed, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(anim != null) {
            anim.Animation = null;
        }

        Vector3 targetPos;

        if(open) {
            open = false;
            targetPos = new Vector3(transform.position.x, Closed, transform.position.z);
        } 
        else {
            open = true;
            targetPos = new Vector3(transform.position.x, Open, transform.position.z);
        }

        IEnumerator ie = DuelManager.Instance.AM.SimpleTranslate(
            transform,
            targetPos,
            Duration,
            InterpolationMode.Linear
        );
        
        anim = new QueueableAnimation(ie, 0f);
        DuelManager.Instance.AM.Play(anim.Animation);
    }
}
