using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideosButtons : MonoBehaviour
{
    [SerializeField]
    VideoPlayer myVideoPlayer;
    public GameObject ButtonPanel;

    // Start is called before the first frame update
    void Start()
    {
        myVideoPlayer.loopPointReached += DoSomethingWhenVideoFinish;
    }

    void DoSomethingWhenVideoFinish(VideoPlayer vp)
    {
        ButtonPanel.SetActive(true);
    }
}
