using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text secondsLabel;

    private DamageCounter damageCounter;

    private void Start()
    {
        damageCounter = FindObjectOfType<DamageCounter>();
    }

    private void Update()
    {
        if (damageCounter)
        {
            secondsLabel.text = Mathf.RoundToInt(damageCounter.TimeLeft).ToString();
        }
        else
        {
            secondsLabel.text = "--";
        }
    }
}
