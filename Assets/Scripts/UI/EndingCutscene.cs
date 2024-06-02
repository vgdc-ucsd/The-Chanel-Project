using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject fmodAudio;

    void Awake() {
        StartCoroutine(WaitForVideoStart());
        fmodAudio.SetActive(true);
        StartCoroutine(WaitForVideoFinish(videoPlayer.clip.length));
    }

    IEnumerator WaitForVideoStart() {
        while(!videoPlayer.isPlaying) {
            yield return null;
        }
    }

    IEnumerator WaitForVideoFinish(double time) {
        yield return new WaitForSeconds((float)time);
        SceneManager.LoadScene(MenuScript.TITLE_INDEX);
    }
}
