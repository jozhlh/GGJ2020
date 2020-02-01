using System.Collections.Generic;
using UnityEngine;

public class Cosmonaut : MonoBehaviour {
    private Rigidbody2D rigidbody;

    [SerializeField]
    private float thrustSpeed = 1000f;

    [SerializeField]
    private AnimationCurve boostCurve = AnimationCurve.EaseInOut(0, 5000f, 1, 0f);
    private float boostTime => this.boostCurve.keys[this.boostCurve.keys.Length - 1].time;
    private float? boostStarted;

    private float lastBoost = -999;

    [SerializeField]
    private float boostCooldown = 3f;

    private List<GravitySource> gravitySources;

    private void Awake() {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.gravitySources = new List<GravitySource>();
    }

    private void FixedUpdate() {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var moveDir = new Vector2(x, y).normalized;

        var startJump = !this.boostStarted.HasValue
            && Input.GetAxis("Jump") > 0
            && Time.time > lastBoost + boostCooldown;
        if (startJump) {
            this.boostStarted = Time.time;
            this.lastBoost = Time.time;
        }

        if (this.boostStarted.HasValue) {
            var boostProgress = (Time.time - this.boostStarted.Value) / this.boostTime;

            if (boostProgress > 1) {
                this.boostStarted = null;
            } else {
                var boost = moveDir * this.boostCurve.Evaluate(boostProgress) * Time.fixedDeltaTime;
                this.rigidbody.AddForce(boost);
            }
        } else {
            var thrust = moveDir * this.thrustSpeed * Time.fixedDeltaTime;
            this.rigidbody.AddForce(thrust);
        }

        foreach (var gravitySource in this.gravitySources) {
            var gravityDir = gravitySource.transform.position - this.transform.position;
            var gravityStrength = gravitySource.GetStrengthAt(this.transform.position);
            var gravity = new Vector2(gravityDir.x, gravityDir.y) * gravityStrength * Time.fixedDeltaTime;

            this.rigidbody.AddForce(gravity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource)) {
            this.gravitySources.Add(gravitySource);
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource)) {
            this.gravitySources.Remove(gravitySource);
        }
    }
}