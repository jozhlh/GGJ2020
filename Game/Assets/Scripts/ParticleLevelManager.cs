using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLevelManager : MonoBehaviour
{
    [SerializeField] private float[] m_particleSizes;
    [SerializeField] private float[] m_particleSpeeds;
    [SerializeField] private float[] m_simulationSpeeds;

    [SerializeField] private ParticleSystem m_particles;

    [SerializeField] private Light m_light;
    public int ParticleLevel;
    private int m_previousParticleLevel = -1;
    private int m_maxParticleLevel;

    private void Start()
    {
        m_maxParticleLevel = Mathf.Min(m_particleSizes.Length, m_particleSpeeds.Length, m_simulationSpeeds.Length) - 1;
    }
    private void Update()
    {
        ParticleLevel = Mathf.Min(ParticleLevel, m_maxParticleLevel);
        if (ParticleLevel != m_previousParticleLevel || true)
        {
            m_previousParticleLevel = ParticleLevel;
            if (ParticleLevel >= 0)
            {
                if (!m_particles.isPlaying)
                {
                    m_particles.Play();
                    m_light.enabled = true;
                }
                var main = m_particles.main;
                main.startSize = m_particleSizes[ParticleLevel];
                main.startSpeed = m_particleSpeeds[ParticleLevel];
                main.simulationSpeed = m_simulationSpeeds[ParticleLevel];
            }
            else
            {
                m_particles.Stop();
                m_light.enabled = false;
            }
        }
    }
}
