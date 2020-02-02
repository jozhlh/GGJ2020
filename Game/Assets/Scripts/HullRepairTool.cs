using UnityEngine;

public class HullRepairTool : Tool
{
    [SerializeField]
    private float useRange = 2f;

    [SerializeField]
    private float repairSpeed = 100f;

    [SerializeField] private ParticleSystem m_particles;
    [SerializeField] private Light m_light;

    private LayerMask hazardMask;
    private Collider[] hazardHits = new Collider[4];

    private Cosmonaut user;

    public override bool IsUsing => !!user;

    private void Awake()
    {
        hazardMask = LayerMask.GetMask("Hazards");
        m_particles.Stop();
        m_light.enabled = false;
    }

    public override void BeginUsing(Cosmonaut user)
    {
        this.user = user;
        m_particles.Play();
        m_light.enabled = true;
    }

    public override void StopUsing(Cosmonaut user)
    {
        this.user = null;
        m_light.enabled = false;
        m_particles.Stop();
    }

    private void Update()
    {
        if (!user)
        {
            return;
        }

        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, useRange, hazardHits, hazardMask, QueryTriggerInteraction.Ignore);
        for (var i = 0; i < hitCount; i += 1)
        {
            if (hazardHits[i].GetComponentInParent<Hazard>() is Hazard hazard
                && hazard.Kind == HazardKind.HullBreach)
            {
                hazard.Extinguish(repairSpeed);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, useRange);
    }
}