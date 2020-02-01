using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private const float maxHealth = 200f;
    private const float minHealth = 25f;

    private float health;

    [SerializeField]
    private float growSpeed = 5.0f;

    [SerializeField]
    private new Renderer renderer;

    [SerializeField]
    private new Collider2D collider;

    private void Awake()
    {
        health = Random.Range(minHealth, maxHealth);
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
        }
    }
}