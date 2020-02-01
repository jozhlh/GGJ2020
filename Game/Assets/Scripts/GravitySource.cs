using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField]
    private float strength = 100f;
    public float Strength
    {
        get => strength;
        set => strength = value;
    }

    private Collider2D trigger;

    private void Awake()
    {
        this.trigger = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        trigger.enabled = true;
    }

    private void OnDisable()
    {
        trigger.enabled = false;
    }

    public float GetStrengthAt(Vector2 pos)
    {
        switch (this.trigger)
        {
            case CircleCollider2D circleTrigger:
                {
                    var distSqr = ((Vector2)circleTrigger.transform.position - pos).sqrMagnitude;
                    var radiusSqr = circleTrigger.radius * circleTrigger.radius;
                    var power = distSqr / radiusSqr;

                    return this.strength * power;
                }

            default: return strength;
        }
    }
}