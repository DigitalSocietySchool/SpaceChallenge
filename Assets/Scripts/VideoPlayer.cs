﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
public class VideoPlayer : MonoBehaviour 
{
#if UNITY_ANDROID || UNITY_IOS

#else
     public MovieTexture introvideo;
#endif
    IEnumerator Example()
    {
        //string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "intro video.ogv");
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "convert2.mp4");
        string result = "";
        if (filePath.Contains("://"))
        {
            WWW www = new WWW(filePath);
            yield return www;
            result = www.text;
        }
        else
        {
            result = System.IO.File.ReadAllText(filePath);
        }
        //Debug.Log(filePath);
        PlayVideo(filePath);
    }

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        //StartCoroutine(Example());
        Handheld.PlayFullScreenMovie("convert2.mp4");


#else
        GetComponent<RawImage>().texture = introvideo;
     AudioSource aud = GetComponent<AudioSource>();
        aud.clip = introvideo.audioClip;
        aud.Play();
        introvideo.Play();
#endif

    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS

#else
     if (!introvideo.isPlaying)
        {
            SceneManager.LoadScene("_Main");
        }
#endif

    }

    void PlayVideo(string videoPath)
    {
        StartCoroutine(PlayVideoCoroutine(videoPath));
    }

    IEnumerator PlayVideoCoroutine(string videoPath)
    {
        Handheld.PlayFullScreenMovie("convert2.mp4", Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene("_Main");
    }
}
*/