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

    [SerializeField]
    private SFXController m_sfxController;
    private Tool heldTool;

    [SerializeField]
    private FloatingBody floatingObject;
    public FloatingBody FloatingObject => floatingObject;

    [SerializeField]
    private CharacterVisuals m_visuals;

    [SerializeField] private ParticleLevelManager m_particles;

    public Transform HandPosition => m_visuals.Hand;

    private Coroutine currentGrab;
    private Coroutine currentDeath;

    [SerializeField]
    private float grabPullRadius = 1f;

    [SerializeField]
    private float grabPullForce = 1000f;

    private readonly List<Tool> pullingTools = new List<Tool>();

    [SerializeField]
    private float grabCatchRadius = 3f;

    private int m_playerIndex = 0;

    public int PlayerIndex { get => m_playerIndex; set => m_playerIndex = value; }

    private void Awake()
    {
        if (!this.floatingObject)
        {
            this.floatingObject = GetComponent<FloatingBody>();
        }
    }

    private void OnDisable()
    {
        currentGrab = null;
    }

    private void Update()
    {
        if (currentDeath != null)
        {
            return;
        }

        bool useTool;

        if (useKeyboard)
        {
            useTool = Input.GetButton("Fire2");
        }
        else
        {
            useTool = playerController.Interact;
        }

        if (IsInputGrabbing() && currentGrab == null)
        {
            m_sfxController.PlayIndex(0);
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

    public void SetupVisuals( GameObject headPrefab, Color color )
    {
        m_visuals.Setup( headPrefab, color );
    }

    private bool IsInputGrabbing()
    {
        return useKeyboard
            ? Input.GetButton("Fire1")
            : playerController.Grab;
    }

    // grabbing state machine. when grabbing, attract nearby objects until
    // one is in grab range, then as long as the player is still holding the button,
    // grab it to make it the held tool.
    // then, the player has to release and press again to drop it
    private IEnumerator Grab()
    {
        var hits = new Collider[10];
        if (!heldTool)
        {
            Tool grabbedTool = null;
            while (!grabbedTool && IsInputGrabbing())
            {
                pullingTools.Clear();

                var toolMask = LayerMask.GetMask("Tools");
                var pullHits = Physics.OverlapSphereNonAlloc(transform.position, grabPullRadius,
                    hits, toolMask, QueryTriggerInteraction.Ignore);

                for (var i = 0; i < pullHits; ++i)
                {
                    if (hits[i].GetComponentInParent<Tool>() is Tool tool)
                    {
                        pullingTools.Add(tool);
                    }
                }

                var catchHits = Physics.OverlapSphereNonAlloc(transform.position, grabCatchRadius,
                    hits,toolMask, QueryTriggerInteraction.Ignore);

                for (var i = 0; i < catchHits; ++i)
                {
                    if (hits[i].GetComponentInParent<Tool>() is Tool tool)
                    {
                        m_sfxController.PlayIndex(1);
                        grabbedTool = tool;
                        break;
                    }
                }
                if (grabbedTool)
                {
                    break;
                }

                yield return null;
            }

            pullingTools.Clear();

            if (grabbedTool != null)
            {
                heldTool = grabbedTool;
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

            m_sfxController.PlayIndex(2);
            heldTool.UnGrab(this);
            heldTool = null;

            // don't allow another grab to start until the button is released again
            while (IsInputGrabbing())
            {
                yield return null;
            }
        }
        else
        {
            m_sfxController.PlayIndex(-1);
        }

        currentGrab = null;
    }

    private void FixedUpdate()
    {
        if (!GameEvents.InGame)
        {
            return;
        }
        float x;
        float y;
        bool jump;
        Vector3 aim = Vector3.zero;

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
            aim.x = playerController.Aim.x;
            aim.y = playerController.Aim.y;
            aim.z = 0.0f;
            aim = aim.normalized;
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

                if(m_particles)
                {
                    m_particles.ParticleLevel = -1;
                }
            }
            else
            {
                if (moveDir.sqrMagnitude < 0.5f)
                {
                    moveDir = Vector2.up;
                }
                var boost = moveDir * this.boostCurve.Evaluate(boostProgress) * Time.fixedDeltaTime;
                this.floatingObject.Rigidbody.AddForce(boost);

                if(m_particles)
                {
                    m_particles.ParticleLevel = 0;
                }
            }
        }
        else
        {
            var thrust = moveDir * this.thrustSpeed * Time.fixedDeltaTime;
            this.floatingObject.Rigidbody.AddForce(thrust);

            if(m_particles)
            {
                m_particles.ParticleLevel = -1;
            }

        }

        // pull tools towards us while in grab+pull mode
        foreach (var tool in pullingTools)
        {
            var dir = (transform.position - tool.transform.position).normalized;
            tool.FloatingBody.Rigidbody.AddForce(dir * grabPullForce * Time.deltaTime);
        }
    }

    public void Die()
    {
        if (currentDeath == null)
        {
            Debug.LogFormat("{0} died", name);
            currentDeath = StartCoroutine(DeathAnimation());
        }
    }

    private IEnumerator DeathAnimation()
    {
        if (currentGrab != null)
        {
            StopCoroutine(currentGrab);
        }

        //floatingObject.enabled = false;
        floatingObject.Rigidbody.isKinematic = true;

        if (heldTool != null)
        {
            heldTool.UnGrab( this );
            heldTool = null;
        }

        var deathAnimation = m_visuals.DeathAnimation();
        while (deathAnimation.MoveNext())
        {
            yield return deathAnimation.Current;
        }

        floatingObject.Rigidbody.isKinematic = false;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        currentDeath = null;

        GameEvents.OnPlayerDied(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabCatchRadius);
        Gizmos.DrawWireSphere(transform.position, grabPullRadius);
    }
}
