using UnityEngine;

public class FireExtinguisher : Tool
{
    [SerializeField]
    private float pushForce = 500f;

    [SerializeField]
    private float spraySize = 2f;

    [SerializeField]
    private GameObject spray;

    [SerializeField] private ParticleSystem m_sprayParticles;

    [SerializeField]
    private Transform sprayCenter;

    private Collider[] sprayHits = new Collider[4];
    private LayerMask hazardMask;

    [SerializeField]
    private float sprayPower = 100f;

    private Cosmonaut currentUser;

    private void Awake()
    {
        //spray.gameObject.SetActive(false);
        m_sprayParticles.Stop();
        hazardMask = LayerMask.GetMask("Hazards");
    }

    public override void BeginUsing(Cosmonaut user)
    {
        Debug.Assert(!currentUser);
        currentUser = user;

        m_sprayParticles.Play();
        //spray.gameObject.SetActive(true);
    }

    public override void StopUsing(Cosmonaut user)
    {
        currentUser = null;

        m_sprayParticles.Stop();
        //spray.gameObject.SetActive(false);
    }

    public override bool IsUsing
    {
        get => !!currentUser;
    }

    private void Update()
    {
        if (currentUser)
        {
            var sprayCount = Physics.OverlapSphereNonAlloc(sprayCenter.position, spraySize,
                sprayHits, hazardMask);
            for (var i = 0; i < sprayCount; i += 1)
            {
                if (sprayHits[i].TryGetComponent<Hazard>(out var hazard)
                    && hazard.Kind == HazardKind.Fire)
                {
                    hazard.Extinguish(sprayPower * Time.deltaTime);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!currentUser)
        {
            return;
        }

        currentUser.FloatingObject.Rigidbody.AddForce(transform.right * pushForce * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        if (sprayCenter)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(sprayCenter.position, spraySize);
        }
    }
}