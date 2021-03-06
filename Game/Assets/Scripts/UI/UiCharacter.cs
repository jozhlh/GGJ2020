﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCharacter : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve m_easing = null;

    [SerializeField]
    private float m_easeDuration = 1.0f;


    [SerializeField]
    private Transform m_targetTransform = null;

    [SerializeField]
    private Transform m_character = null;

    [SerializeField]
    private Transform m_headAttachPoint = null;

    [SerializeField]
    private GameObject m_currentHead = null;

    [SerializeField]
    private SkinnedMeshRenderer m_rend = null;

    private void Start()
    {
        GameEvents.OnRoundStart += IDontNeedYouNoMore;
    }

    private void OnDestroy()
    {
        GameEvents.OnRoundStart -= IDontNeedYouNoMore;
    }

    void IDontNeedYouNoMore()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    public void Join( Color color )
    {
        if (m_rend)
        {
            var mats = m_rend.materials;
            mats[0].SetColor("_FresnelColour", color);
            mats[1].SetColor("_BaseColor", color);
            mats[1].SetColor("_EmissionColor", color);
            mats[1].SetColor("_EmissiveColor", color);
            mats[1].SetFloat("_EmissiveIntensity", 3.0f);
            m_rend.materials = mats;
        }

        ColorHead(color);

        StartCoroutine(JoinRoutine());
    }

    // Update is called once per frame
    public void ChangeHead( GameObject head, Color color )
    {
        Destroy(m_currentHead);
        m_currentHead = Instantiate( head, m_headAttachPoint, false );
        m_currentHead.name = "Helmet_Temp";
        m_currentHead.transform.localPosition = Vector3.zero;
        m_currentHead.transform.localRotation = Quaternion.identity;

        if (m_rend)
        {
            var mat = m_rend.material;
            var colr = mat.GetColor("_FresnelColour");
            //Debug.Log($"OG: {colr}");
            mat.SetColor("_FresnelColour", color);
            m_rend.material = mat;
        }

        ColorHead(color);
    }


    private void ColorHead( Color color )
    {
        var headRend = m_currentHead.GetComponentInChildren<MeshRenderer>();
        var headMat = headRend.material;
        headMat.SetColor("_FresnelColour", color);
        headRend.material = headMat;
    }


    private IEnumerator JoinRoutine()
    {
        var startPos = m_character.position;
        var target = m_targetTransform.position;
        var count = 0.0f;
        var t = 0.0f;

        while (count < m_easeDuration)
        {
            t = m_easing.Evaluate(count / m_easeDuration);
            m_character.position = Vector3.Lerp( startPos, target, t);
            count += Time.deltaTime;

            yield return null;
        }

        m_character.position = target;
    }
}
