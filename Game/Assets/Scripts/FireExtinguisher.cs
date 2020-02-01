using UnityEngine;

public class FireExtinguisher : Tool
{
    [SerializeField]
    private Vector2 pushForce = new Vector2(-500f, 0f);

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

        var worldPushForce = transform.localToWorldMatrix.MultiplyVector(pushForce);
        currentUser.FloatingObject.Rigidbody.AddForce(worldPushForce * Time.fixedDeltaTime);
    }
}