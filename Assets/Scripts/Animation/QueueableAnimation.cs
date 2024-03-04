using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueableAnimation
{
    public IEnumerator Animation;
    public float Delay;

    public QueueableAnimation(IEnumerator animation, float delay) {
        Animation = animation;
        Delay = delay;
    }
}
