using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private float m_smoothValue = 0.2f;

    [SerializeField] private Transform m_toolAttachPoint = null;

    public Transform Hand => m_toolAttachPoint;

    private Vector2 m_lookAtTarget = Vector2.right;


    // Update is called once per frame
    void Update()
    {
        var sideLook = Vector2.Lerp(transform.right, m_lookAtTarget, m_smoothValue);
        transform.right = sideLook.normalized;
        Debug.DrawLine(transform.position, transform.position + transform.right );
    }

    public void SetLookDirection( Vector3 lookDirection )
    {
        m_lookAtTarget = lookDirection;
    }
}
