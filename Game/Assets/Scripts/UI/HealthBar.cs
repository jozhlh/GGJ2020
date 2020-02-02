using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image[] healthBarFill = new Image[2];

    [SerializeField] private AudioSource m_audioSource;

    private DamageCounter damageCounter;

    private void Awake()
    {
        GameEvents.OnDamage += OnDamage;

        damageCounter = FindObjectOfType<DamageCounter>();
        if (!damageCounter)
        {
            Debug.LogError("missing damage counter");
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnDamage -= OnDamage;
        m_audioSource.Stop();
    }

    private void OnDamage(int damage)
    {
        var bar = healthBarFill[0];
        float prevFill = bar.fillAmount;
        healthBarFill[0].fillAmount = Mathf.InverseLerp(0f, DamageCounter.MaxHealth, damageCounter.Health);
        healthBarFill[1].fillAmount = healthBarFill[0].fillAmount;

        if (bar.fillAmount < 0.1f && prevFill > 0.1f)
        {
            m_audioSource.Play();
        }
        else if (bar.fillAmount > 0.1f && prevFill < 0.1f)
        {
            m_audioSource.Stop();
        }
    }
}
