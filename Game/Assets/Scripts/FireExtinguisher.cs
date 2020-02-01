using UnityEngine;

public class FireExtinguisher : Tool
{
    [SerializeField]
    private Vector2 pushForce = new Vector2(-500f, 0f);

    [SerializeField]
    private Vector2 spraySize = new Vector2(3f, 0.5f);

    [SerializeField]
    private GameObject sprayFx;

    [SerializeField]
    private Collider2D sprayCollider;
    private ContactPoint2D[] sprayContacts = new ContactPoint2D[8];

    private Cosmonaut currentUser;

    private void Awake()
    {
        sprayFx.gameObject.SetActive(false);
    }

    public override void BeginUsing(Cosmonaut user)
    {
        Debug.Assert(!currentUser);
        currentUser = user;

        sprayFx.gameObject.SetActive(true);
    }

    public override void StopUsing(Cosmonaut user)
    {
        currentUser = null;

        sprayFx.gameObject.SetActive(false);
    }

    public override bool IsUsing
    {
        get => !!currentUser;
    }

    private void Update()
    {
        if (!currentUser)
        {
            return;
        }

        var hitCount = sprayCollider.GetContacts(sprayContacts);
        for (var hit = 0; hit < hitCount; hit += 1)
        {
            if (sprayContacts[hit].collider.TryGetComponent<Fire>(out var fire))
            {
                fire.Extinguish();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!currentUser)
        {
            return;
        }

        currentUser.FloatingObject.Rigidbody.AddForce(pushForce * Time.fixedDeltaTime);
    }
}