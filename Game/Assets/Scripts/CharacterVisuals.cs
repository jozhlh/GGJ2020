﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private float m_rotateSpeed = 150.0f;

    [SerializeField] private float m_lerpSpeed = 0.4f;

    [SerializeField] private float m_rayDistance = 0.3f;

    [SerializeField] private Transform m_headPoint = null;

    [SerializeField] private Transform m_toolAttachPoint = null;

    [SerializeField] private Transform m_parent = null;

    [SerializeField] private Rigidbody m_rb = null;

    [SerializeField] private PlayerController m_input = null;

    [SerializeField] private Animator m_animator = null;

    [SerializeField] private SkinnedMeshRenderer m_rend = null;

    [SerializeField] private Color m_deathColor = Color.red;

    [SerializeField] private float m_deathFresnelPower = -1f;

    [SerializeField]
    private Light[] m_lights = new Light[2];

    private MeshRenderer m_headRend;

    public Transform Hand => m_toolAttachPoint;

    private Vector2 m_lookAtTarget = Vector2.right;

    private float m_yTarget = 0.0f;

    private Color m_color;


    // Update is called once per frame
    void Update()
    {
        if (m_input.Move.x > 0.5f)
        {
            var rot = transform.eulerAngles;
            var y = Mathf.LerpAngle( rot.y, 95.0f, m_lerpSpeed );
            transform.eulerAngles = new Vector3( 0.0f, y, 0.0f);
        }

        if (m_input.Move.x < -0.5f)
        {
            var rot = transform.eulerAngles;
            var y = Mathf.LerpAngle( rot.y, 265.0f, m_lerpSpeed );
            transform.eulerAngles = new Vector3( 0.0f, y, 0.0f);
        }

        if (m_input.Aim.x > 0.5f)
        {
            m_toolAttachPoint.RotateAround( m_parent.transform.position, Vector3.forward, -m_rotateSpeed * Time.deltaTime);
        }

        if (m_input.Aim.x < -0.5f)
        {
            m_toolAttachPoint.RotateAround( m_parent.transform.position, Vector3.forward, m_rotateSpeed * Time.deltaTime);
        }
        // var sideLook = Vector2.Lerp(transform.right, m_lookAtTarget, m_smoothValue);
        // transform.right = sideLook.normalized;
        // Debug.DrawLine(transform.position, transform.position + transform.right );

        if (m_animator)
        {
            UpdateAnimator();
        }
    }

    public void UpdateAnimator( )
    {
        var velocity = m_rb.velocity;
        m_animator.SetFloat("HorizontalVelocity", Mathf.Abs(velocity.x));
        m_animator.SetFloat("VerticalVelocity", velocity.y);

        velocity.y = 0.0f;
        var move = m_input.Move;
        move.y = 0.0f;

        m_animator.SetFloat("xDot", Vector3.Dot(velocity, move));

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, m_rayDistance, LayerMask.GetMask("Env_Collision")))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            m_animator.SetBool("isGrounded", true);
        }
        else
        {
            m_animator.SetBool("isGrounded", false);
        }
    }


    public void Setup( GameObject headPrefab, Color color )
    {
        var head = Instantiate( headPrefab, m_headPoint, false);
        head.name = "Helmet_Temp";
        head.transform.localPosition = Vector3.zero;
        head.transform.localRotation = Quaternion.identity;
        m_color = color;

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

        m_headRend = head.GetComponentInChildren<MeshRenderer>();
        var headMat = m_headRend.material;
        headMat.SetColor("_FresnelColour", color);
        m_headRend.material = headMat;

        foreach( var light in m_lights)
        {
            light.color = color;
        }
    }

    public IEnumerator DeathAnimation()
    {
        var renderers = new Renderer[] { m_rend, m_headRend };

        var fresnelColorProp = Shader.PropertyToID("_FresnelColour");
        var fresnelPowerProp = Shader.PropertyToID("_FresnelPower");

        var startFresnelColor = renderers[0].material.GetColor(fresnelColorProp);
        var startFresnelPower = renderers[0].material.GetFloat(fresnelPowerProp);

        const float deathDuration = 1.0f;
        var startTime = Time.time;

        float deathProgress;
        do
        {
            deathProgress = (Time.time - startTime) / deathDuration;
            foreach (var renderer in renderers)
            {
                var color = Color.LerpUnclamped(startFresnelColor, m_deathColor, deathProgress);
                var fresnelPower = Mathf.LerpUnclamped(startFresnelPower, m_deathFresnelPower, deathProgress);
                renderer.material.SetColor(fresnelColorProp, color);
                renderer.material.SetFloat(fresnelPowerProp, fresnelPower);
            }
            yield return null;
        }
        while (deathProgress < 1);

        foreach (var renderer in renderers)
        {
            renderer.material.SetColor(fresnelColorProp, startFresnelColor);
            renderer.material.SetFloat(fresnelPowerProp, startFresnelPower);
        }
    }
}
