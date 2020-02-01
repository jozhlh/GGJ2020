using System.Runtime.CompilerServices;
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

    protected virtual void Awake()
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

    public (float strength, Vector3 gravityTarget) GetStrengthAndTargetAt(Vector2 pos)
    {
        switch (this.trigger)
        {
            case CircleCollider2D circleTrigger:
                {
                    var distSqr = ((Vector2)circleTrigger.transform.position - pos).sqrMagnitude;
                    var radiusSqr = circleTrigger.radius * circleTrigger.radius;
                    var power = distSqr / radiusSqr;

                    return (this.strength * power, transform.position);
                }
            case BoxCollider2D boxTrigger:
                {
                    var cachedTransform = transform;
                    float halfScaleX = cachedTransform.lossyScale.x / 2f;
                    float halfScaleY = cachedTransform.lossyScale.y / 2f;
                    float posX = cachedTransform.position.x;
                    float posY = cachedTransform.position.y;

                    if (halfScaleX > halfScaleY)
                    {
                        // is floor or ceiling

                        float leftX = posX- halfScaleX;
                        float rightX = posX + halfScaleX;
                        float distanceToFloor = FindDistanceToSegment (pos, new Vector2(leftX, posY), new Vector2(rightX, posY), out var closest);
                        return (this.strength / distanceToFloor, new Vector3(closest.x, closest.y, 0f));
                    }
                    else
                    {
                        // is wall
                        float topY = posY + halfScaleY;
                        float bottomY = posY - halfScaleY;
                        float distanceToWall = FindDistanceToSegment (pos, new Vector2(posX, topY), new Vector2(posX, bottomY), out var closest);
                        return (this.strength / distanceToWall, new Vector3(closest.x, closest.y, 0f));
                    }
                }

            default: return (strength, transform.position);
        }
    }

    private float FindDistanceToSegment( Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
    {
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        if ((dx == 0) && (dy == 0))
        {
            // It's a point not a line segment.
            closest = p1;
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        // Calculate the t that minimizes the distance.
        float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) / (dx * dx + dy * dy);

        // See if this represents one of the segment's
        // end points or a point in the middle.
        if (t < 0)
        {
            closest = new Vector2(p1.x, p1.y);
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(p2.x, p2.y);
            dx = pt.x - p2.x;
            dy = pt.y - p2.y;
        }
        else
        {
            closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
            dx = pt.x - closest.x;
            dy = pt.y - closest.y;
        }

        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}