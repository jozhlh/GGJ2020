using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource m_music;
    void Start()
    {
        GameEvents.OnRoundStart += MissionStartedListener;
        GameEvents.OnRoundEnd += MissionEndedListener;
    }

    void OnDestroy()
    {
        GameEvents.OnRoundStart -= MissionStartedListener;
        GameEvents.OnRoundEnd -= MissionEndedListener;
    }

    private void MissionStartedListener()
    {
        m_music.Play();
    }

    private void MissionEndedListener()
    {
        m_music.Stop();
    }
}
