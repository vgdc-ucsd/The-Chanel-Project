using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{
    public VersusScreen vsc;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            VsScreenState state = PersistentData.Instance.VsState;
            vsc.characterAudio.Stop();
            if (state == VsScreenState.VS) {
                SceneManager.LoadScene(MenuScript.DUEL_INDEX);
            }
            else if (state == VsScreenState.Win) {
                SceneManager.LoadScene(MenuScript.REWARD_INDEX);
            }
            else {
                MenuScript.Instance.LoadTitle();
            }
        }
    }
}
