using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField]
    private float strength = 100f;

    private Collider2D trigger;

    private void Awake()
    {
        this.trigger = GetComponent<Collider2D>();
    }

    public float GetStrengthAt(Vector2 pos)
    {
        switch (this.trigger)
        {
            case CircleCollider2D circleTrigger:
                {
                    var distSqr = ((Vector2)circleTrigger.transform.position - pos).sqrMagnitude;
                    var radiusSqr = circleTrigger.radius * circleTrigger.radius;
                    var power = 1f - Mathf.Clamp01(distSqr / radiusSqr);

                    return this.strength * power;
                }

            default: return strength;
        }
    }
}