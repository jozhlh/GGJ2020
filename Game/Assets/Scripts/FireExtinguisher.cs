using UnityEngine;

public class FireExtinguisher : Tool
{
    [SerializeField]
    private float pushForce = 500f;

    [SerializeField]
    private FireExtinguisherSpray spray;

    private Cosmonaut currentUser;

    private void Awake()
    {
        spray.gameObject.SetActive(false);
    }

    public override void BeginUsing(Cosmonaut user)
    {
        Debug.Assert(!currentUser);
        currentUser = user;

        spray.gameObject.SetActive(true);
    }

    public override void StopUsing(Cosmonaut user)
    {
        currentUser = null;

        spray.gameObject.SetActive(false);
    }

    public override bool IsUsing
    {
        get => !!currentUser;
    }

    private void FixedUpdate()
    {
        if (!currentUser)
        {
            return;
        }

        currentUser.FloatingObject.Rigidbody.AddForce(transform.right * pushForce * Time.fixedDeltaTime);
    }
}