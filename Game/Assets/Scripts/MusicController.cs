using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource m_music;

    public static Action<bool> MissionStateChanged = (started) => {};
    void Start()
    {
        MissionStateChanged += MissionStateChangedListener;
    }

    void OnDestroy()
    {
        MissionStateChanged -= MissionStateChangedListener;
    }

    private void MissionStateChangedListener(bool started)
    {
        if (started)
        {
            m_music.Play();
        }
        else
        {
            m_music.Stop();
        }
    }
}
