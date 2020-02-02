using System.Diagnostics;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
public class SFXController : MonoBehaviour
{
    [SerializeField] private string[] m_animationEvents;
    [SerializeField] private int[] m_animEventSfx;
    [SerializeField] private AudioClip[] m_sfx;
    [SerializeField] private AudioSource m_source;

    public void PlayRandom()
    {
        int index = Random.Range(0, m_sfx.Length);
        PlayIndex(index);
    }

    public void PlayIndex(int index)
    {
        if (index == -1)
        {
            m_source.Stop();
            return;
        }
        m_source.clip = m_sfx[index];
        m_source.Play();
    }

    public void OnAnimationEvent(string arg)
    {
        //Debug.Log ($"Found anim event {arg}");
        int eventIndex = -1;
        for (int i = 0; i < m_animationEvents.Length; i++)
        {
            if (m_animationEvents[i].Equals(arg))
            {
                eventIndex = i;
                break;
            }
        }
        if (eventIndex >= 0)
        {
            int sfxIndex = m_animEventSfx[eventIndex];
            PlayIndex(sfxIndex);
        }
    }
}