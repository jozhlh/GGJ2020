using System.Collections;
using UnityEngine;

public enum HazardKind
{
    Fire,
    GasLeak,
    Alien,
}

public class Hazard : MonoBehaviour
{
    private const float damageTickTime = 1.0f;

    private const float minScale = 1.0f;
    private const float maxScale = 3.0f;

    private const float medDamageThreshold = 0.5f;
    private const float bigDamageThreshold = 0.9f;

    private const float maxHealth = 200f;
    private const float minHealth = 25f;

    private float health;

    private float lastDamageTick;

    private MultiplayerManager multiplayer;

    [SerializeField]
    private float growSpeed = 5.0f;

    [SerializeField]
    private GameObject mainVisual;

    [SerializeField]
    private new Collider2D collider;

    [SerializeField]
    private HazardKind kind;
    public HazardKind Kind => kind;

    private void Awake()
    {
        multiplayer = FindObjectOfType<MultiplayerManager>();

        lastDamageTick = Time.time;

        var healthAmount = Random.Range(0f, 0.75f);
        health = Mathf.LerpUnclamped(minHealth, maxHealth, healthAmount);
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

        var allRenderers = GetComponentsInChildren<Renderer>();

        float dieProgress;
        do
        {
            dieProgress = (Time.time - dieStart) / dieTime;

            foreach (var renderer in allRenderers)
            {
                var color = renderer.material.color;
                color.a = 1 - dieProgress;
                renderer.material.color = color;
            }

            var scale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, 1 - dieProgress);
            transform.localScale = scale;

            yield return null;
        }
        while (dieProgress < 1);

        Destroy(gameObject);
    }

    private void Update()
    {
        // don't let fire grow before game begins
        if (multiplayer && multiplayer.ActivePlayerCount == 0)
        {
            return;
        }

        if (health > 0)
        {
            health = Mathf.Min(maxHealth, health + growSpeed * Time.deltaTime);

            var healthProgress = Mathf.InverseLerp(minHealth, maxHealth, Mathf.Max(minHealth, health));
            var scale = Mathf.Lerp(minScale, maxScale, healthProgress);

            mainVisual.transform.localScale = new Vector3(scale, scale, scale);

            if (Time.time - lastDamageTick > damageTickTime
                && kind != HazardKind.Alien)
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