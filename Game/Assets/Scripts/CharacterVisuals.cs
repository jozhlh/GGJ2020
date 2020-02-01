using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private float m_rotateSpeed = 20.0f;

    [SerializeField] private Transform m_toolAttachPoint = null;

    [SerializeField] private Rigidbody2D m_rigidBody;

    [SerializeField] private PlayerController m_input;

    public Transform Hand => m_toolAttachPoint;

    private Vector2 m_lookAtTarget = Vector2.right;


    // Update is called once per frame
    void Update()
    {
        if (m_input.Move.x > 0.2f)
        {
            transform.forward = Vector3.right;
        }

        if (m_input.Move.x < 0.2f)
        {
            transform.forward = -Vector3.right;
        }

        

        if (m_input.Aim.x > 0.1f)
        {
            m_toolAttachPoint.RotateAround( m_rigidBody.transform.position, Vector3.forward, m_rotateSpeed * Time.deltaTime);
        }

        if (m_input.Aim.x < 0.1f)
        {
            m_toolAttachPoint.RotateAround( m_rigidBody.transform.position, Vector3.forward, -m_rotateSpeed * Time.deltaTime);
        }
        // var sideLook = Vector2.Lerp(transform.right, m_lookAtTarget, m_smoothValue);
        // transform.right = sideLook.normalized;
        // Debug.DrawLine(transform.position, transform.position + transform.right );
    }

    public void SetLookDirection( Vector3 lookDirection )
    {
        // m_lookAtTarget = lookDirection;
    }
}
