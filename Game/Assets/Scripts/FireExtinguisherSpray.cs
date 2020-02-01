using UnityEngine;

public class FireExtinguisherSpray : MonoBehaviour
{
    [SerializeField]
    private float extinguishSpeed = 20.0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<Hazard>(out var fire)
            && fire.Kind == HazardKind.Fire)
        {
            fire.Extinguish(Time.fixedDeltaTime * extinguishSpeed);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Hazard>(out var fire))
        {
            fire.Extinguish(Time.fixedDeltaTime * extinguishSpeed);
        }
    }
}