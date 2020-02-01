using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private const float damageTickTime = 1.0f;

    private const float medDamageThreshold = 0.5f;
    private const float bigDamageThreshold = 0.9f;

    private const float maxHealth = 200f;
    private const float minHealth = 25f;

    private float health;

    private float lastDamageTick;

    [SerializeField]
    private float growSpeed = 5.0f;

    [SerializeField]
    private new Renderer renderer;

    [SerializeField]
    private new Collider2D collider;

    private void Awake()
    {
        lastDamageTick = Time.time;

        var sizeAmount = Random.Range(0f, 0.75f);
        health = Mathf.LerpUnclamped(minHealth, maxHealth, sizeAmount);
    }

    public void Extinguish(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        collider.enabled = false;

        var dieTime = 0.5f;
        var dieStart = Time.time;

        float dieProgress;
        do
        {
            dieProgress = (Time.time - dieStart) / dieTime;
            var color = renderer.material.color;
            color.a = 1 - dieProgress;
            renderer.material.color = color;

            var scale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, 1 - dieProgress);
            transform.localScale = scale;

            yield return null;
        }
        while (dieProgress < 1);

        Destroy(gameObject);
    }

    private void Update()
    {
        if (health > 0)
        {
            health = Mathf.Min(maxHealth, health + growSpeed * Time.deltaTime);

            var healthProgress = Mathf.InverseLerp(minHealth, maxHealth, Mathf.Max(minHealth, health));
            var scale = Mathf.Lerp(1.0f, 2.0f, healthProgress);

            transform.localScale = new Vector3(scale, scale, scale);

            if (Time.time - lastDamageTick > damageTickTime)
            {
                lastDamageTick = Time.time;

                var damageScale = Mathf.InverseLerp(minHealth, maxHealth, health);
                int damage;
                if (damageScale < medDamageThreshold)
                {
                    damage = 1;
                }
                else if (damageScale < bigDamageThreshold)
                {
                    damage = 2;
                }
                else
                {
                    damage = 3;
                }

                GameEvents.OnDamage(damage);
            }
        }
    }
}