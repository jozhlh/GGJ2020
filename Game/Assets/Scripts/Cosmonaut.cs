using System.Collections.Generic;
using UnityEngine;

public class Cosmonaut : MonoBehaviour
{


    [Header("Settings")]
    [SerializeField]
    private bool useKeyboard = false;

    [SerializeField]
    private float thrustSpeed = 1000f;

    [SerializeField]
    private float boostCooldown = 3f;

    [SerializeField]
    private AnimationCurve boostCurve = AnimationCurve.EaseInOut(0, 5000f, 1, 0f);
    private float boostTime => this.boostCurve.keys[this.boostCurve.keys.Length - 1].time;

    private float? boostStarted;

    private float lastBoost = -999;

    [Header("References")]
    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    private PlayerController playerController;
    public PlayerController PlayerController => playerController;


    private List<GravitySource> gravitySources;

    private int m_playerIndex = 0;

    public int PlayerIndex { get => m_playerIndex; set => m_playerIndex = value; }

    private void Awake()
    {
        if (!this.rigidbody)
        {
            this.rigidbody = GetComponent<Rigidbody2D>();
        }

        this.gravitySources = new List<GravitySource>();
    }

    private void FixedUpdate()
    {
        if (useKeyboard)
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            jump = Input.GetAxis("Jump") > 0;
        }
        else
        {
            playerController.UpdateInput();
            x = playerController.Move.x;
            y = playerController.Move.y;
            jump = playerController.Jump;
        }

        var moveDir = new Vector2(x, y).normalized;

        var startJump = !this.boostStarted.HasValue
            && jump
            && Time.time > lastBoost + boostCooldown;
        if (startJump)
        {
            this.boostStarted = Time.time;
            this.lastBoost = Time.time;
        }

        if (this.boostStarted.HasValue)
        {
            var boostProgress = (Time.time - this.boostStarted.Value) / this.boostTime;

            if (boostProgress > 1)
            {
                this.boostStarted = null;
            }
            else
            {
                var boost = moveDir * this.boostCurve.Evaluate(boostProgress) * Time.fixedDeltaTime;
                this.rigidbody.AddForce(boost);
            }
        }
        else
        {
            var thrust = moveDir * this.thrustSpeed * Time.fixedDeltaTime;
            this.rigidbody.AddForce(thrust);
        }

        foreach (var gravitySource in this.gravitySources)
        {
            var gravityDir = gravitySource.transform.position - this.transform.position;
            var gravityStrength = gravitySource.GetStrengthAt(this.transform.position);
            var gravity = new Vector2(gravityDir.x, gravityDir.y) * gravityStrength * Time.fixedDeltaTime;

            this.rigidbody.AddForce(gravity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource))
        {
            this.gravitySources.Add(gravitySource);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<GravitySource>(out var gravitySource))
        {
            this.gravitySources.Remove(gravitySource);
        }
    }
}