using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopCardTween : MonoBehaviour
{
    [SerializeField]
    private float animCycle = 3.5f;
    [SerializeField]
    private float offset = 600f;
    [SerializeField]
    private float elasticAmp = 0.1f;
    [SerializeField]
    [Tooltip ("Keep same/close as animCycle")]
    private float elasticDuration = 3.3f;

    void Awake()
    {
        transform.localPosition = new Vector3(transform.position.x - offset, 0, 0);
        transform.DOLocalMove(new Vector3(transform.position.x + offset, 0, 0), animCycle).SetEase(Ease.OutElastic, elasticAmp, elasticDuration);
    }
}
