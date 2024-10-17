using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerInit : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        // Set the video player to autoplay
        videoPlayer.playOnAwake = true;

        // Check if autoplay works (for browsers that allow muted autoplay)
        videoPlayer.Play();

        // Add a listener for interaction to start playback if autoplay is blocked
        Application.focusChanged += OnApplicationFocus;
    }

    // When user focuses or interacts with the page, start the video
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }
}
