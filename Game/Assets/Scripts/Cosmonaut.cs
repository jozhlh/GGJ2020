using System.Collections;
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
    private PlayerController playerController;
    public PlayerController PlayerController => playerController;

    public Vector2 HandPosition => (Vector2)transform.position + Vector2.right * 0.1f;

    private Tool heldTool;

    private readonly List<Tool> grabbableTools = new List<Tool>();

    [SerializeField]
    private FloatingBody floatingObject;
    public FloatingBody FloatingObject => floatingObject;

    [SerializeField]
    private GravitySource grabGravity;
    private Coroutine currentGrab;

    private float grabStrength;

    private int m_playerIndex = 0;

    public int PlayerIndex { get => m_playerIndex; set => m_playerIndex = value; }

    private void Awake()
    {
        if (!this.floatingObject)
        {
            this.floatingObject = GetComponent<FloatingBody>();
        }

        grabStrength = grabGravity.Strength;
        grabGravity.Strength = 0;
    }

    private void OnDisable()
    {
        currentGrab = null;
        grabGravity.Strength = 0;
    }

    private void Update()
    {
        bool useTool;

        if (useKeyboard)
        {
            useTool = Input.GetButton("Fire2");
        }
        else
        {
            useTool = false;
        }

        if (IsInputGrabbing() && currentGrab == null)
        {
            currentGrab = StartCoroutine(Grab());
        }

        if (heldTool)
        {
            if (heldTool.IsUsing && !useTool)
            {
                heldTool.StopUsing(this);
            }
            else if (!heldTool.IsUsing && useTool)
            {
                heldTool.BeginUsing(this);
            }
        }
    }

    private bool IsInputGrabbing()
    {
        return useKeyboard
            ? Input.GetButton("Fire1")
            : playerController.Interact;
    }

    // grabbing state machine. when grabbing, attract nearby objects until
    // one is in grab range, then as long as the player is still holding the button,
    // grab it to make it the held tool.
    // then, the player has to release and press again to drop it
    private IEnumerator Grab()
    {
        if (!heldTool)
        {
            grabGravity.Strength = grabStrength;

            while (grabbableTools.Count == 0 && IsInputGrabbing())
            {
                yield return null;
            }

            grabGravity.Strength = 0;

            if (grabbableTools.Count > 0)
            {
                heldTool = grabbableTools[0];
                heldTool.Grab(this);

                // wait after pickup for release and press
                while (IsInputGrabbing())
                {
                    yield return null;
                }
            }
        }

        // wait for grab button to be pressed again to release the held tool
        if (heldTool)
        {
            while (!IsInputGrabbing())
            {
                yield return null;
            }

            heldTool.UnGrab(this);
            heldTool = null;

            // don't allow another grab to start until the button is released again
            while (IsInputGrabbing())
            {
                yield return null;
            }
        }

        currentGrab = null;
    }

    private void FixedUpdate()
    {
        float x;
        float y;
        bool jump;

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
                this.floatingObject.Rigidbody.AddForce(boost);
            }
        }
        else
        {
            var thrust = moveDir * this.thrustSpeed * Time.fixedDeltaTime;
            this.floatingObject.Rigidbody.AddForce(thrust);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<Tool>() is Tool tool)
        {
            this.grabbableTools.Add(tool);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<Tool>() is Tool tool)
        {
            this.grabbableTools.Remove(tool);
        }
    }
}