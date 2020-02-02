using UnityEngine;

public class HullRepairTool : Tool
{
    [SerializeField]
    private float useRange = 2f;

    [SerializeField]
    private float repairSpeed = 100f;

    private LayerMask hazardMask;
    private Collider[] hazardHits = new Collider[4];

    private Cosmonaut user;

    public override bool IsUsing => !!user;

    private void Awake()
    {
        hazardMask = LayerMask.GetMask("Hazards");
    }

    public override void BeginUsing(Cosmonaut user)
    {
        this.user = user;
    }

    public override void StopUsing(Cosmonaut user)
    {
        this.user = null;
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