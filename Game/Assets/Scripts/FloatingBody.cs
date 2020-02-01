using System.Collections.Generic;
using UnityEngine;

public class FloatingBody : MonoBehaviour
{
    [SerializeField]
    private float mass = 1.0f;

    [SerializeField]
    private RigidbodyConstraints constraints;

    [SerializeField]
    private Rigidbody m_rigidbody;
    public Rigidbody Rigidbody => m_rigidbody;

    private readonly List<GravitySource> gravitySources = new List<GravitySource>();

    private void Awake()
    {
    }

    private void OnEnable()
    {
        //rigidbody = gameObject.AddComponent<Rigidbody>();
        //m_rigidbody.isKinematic = false;
        // rigidbody.mass = mass;
        // rigidbody.constraints = constraints
        //     | RigidbodyConstraints.FreezePositionZ
        //     | RigidbodyConstraints.FreezeRotationY;
        // rigidbody.useGravity = false;
    }

    private void OnDisable()
    {
        // if (m_rigidbody)
        // {
        //    // m_rigidbody.isKinematic =true;
        //     // Destroy(rigidbody);
        //     // rigidbody = null;
        // }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource))
        {
            if (!gravitySource.transform.IsChildOf(transform))
            {
                this.gravitySources.Add(gravitySource);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource))
        {
            this.gravitySources.Remove(gravitySource);
        }
    }

    private void FixedUpdate()
    {
        foreach (var gravitySource in this.gravitySources)
        {
            var (gravityStrength, gravityTarget) = gravitySource.GetStrengthAndTargetAt(this.transform.position);
            var gravityDir = gravityTarget - this.transform.position;

            var gravity = new Vector2(gravityDir.x, gravityDir.y) * gravityStrength * Time.fixedDeltaTime;

            this.m_rigidbody.AddForce(gravity);
        }
    }
}