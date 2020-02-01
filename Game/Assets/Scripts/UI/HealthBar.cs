using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBarFill;

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

    private void OnDamage(int damage)
    {
        healthBarFill.fillAmount = Mathf.InverseLerp(0f, DamageCounter.MaxHealth, damageCounter.Health);
    }
}
