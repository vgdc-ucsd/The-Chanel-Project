using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoUrlUpdater : MonoBehaviour
{
    public string FileName;

    void Start() {
        GetComponent<VideoPlayer>().url = System.IO.Path.Combine (Application.streamingAssetsPath, FileName);
    }
}
