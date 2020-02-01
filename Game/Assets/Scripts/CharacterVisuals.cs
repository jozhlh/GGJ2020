using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    [SerializeField] private float m_rotateSpeed = 150.0f;

    [SerializeField] private float m_lerpSpeed = 0.4f;

    [SerializeField] private Transform m_toolAttachPoint = null;

    [SerializeField] private Transform m_parent;

    [SerializeField] private PlayerController m_input;

    public Transform Hand => m_toolAttachPoint;

    private Vector2 m_lookAtTarget = Vector2.right;

    private float m_yTarget = 0.0f;


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
    }

    public void SetLookDirection( Vector3 lookDirection )
    {
        // m_lookAtTarget = lookDirection;
    }
}
