using System.Collections.Generic;
using UnityEngine;

public class FloatingBody : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody2D rigidbody;
    public Rigidbody2D Rigidbody => rigidbody;

    private readonly List<GravitySource> gravitySources = new List<GravitySource>();

    private void Awake()
    {
        rigidbody.gravityScale = 0;
    }

    private void OnEnable()
    {
        rigidbody.simulated = true;
    }

    private void OnDisable()
    {
        rigidbody.simulated = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource))
        {
            if (!gravitySource.transform.IsChildOf(transform))
            {
                this.gravitySources.Add(gravitySource);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
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
            var gravityDir = gravitySource.transform.position - this.transform.position;
            var gravityStrength = gravitySource.GetStrengthAt(this.transform.position);
            var gravity = new Vector2(gravityDir.x, gravityDir.y) * gravityStrength * Time.fixedDeltaTime;

            this.rigidbody.AddForce(gravity);
        }
    }
}