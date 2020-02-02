using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private float m_gizmoSize;

    [SerializeField] private AnimationCurve m_spawnRateOverTime = AnimationCurve.Linear(0, 5, 1, 10);

    [Space(10)]
    [SerializeField] private GameObject[] m_spawnPrefabs;

    [SerializeField] private float m_tickRate;
    private float m_lastTick;

    private DamageCounter m_damageCounter;

    private GameObject[] m_slots;
    private List<int> m_freeSlots;

    private void Start()
    {
        m_damageCounter = FindObjectOfType<DamageCounter>();

        m_slots = new GameObject[transform.childCount];
        m_freeSlots = new List<int>(m_slots.Length);
    }

    private void Update()
    {
        if (!m_damageCounter || !GameEvents.InGame)
        {
            m_lastTick = Time.time;
            return;
        }

        if (Time.time - m_lastTick < m_tickRate)
        {
            return;
        }

        m_lastTick += m_tickRate;

        var timeProgress = Mathf.InverseLerp(DamageCounter.MaxTime, 0, m_damageCounter.TimeLeft);
        var targetCount = Mathf.CeilToInt(m_spawnRateOverTime.Evaluate(timeProgress));

        m_freeSlots.Clear();
        for (var i = 0; i < m_slots.Length; i += 1)
        {
            if (!m_slots[i])
            {
                m_freeSlots.Add(i);
            }
        }

        var currentCount = m_slots.Length - m_freeSlots.Count;
        var missing = Mathf.Clamp(targetCount, 0, m_slots.Length) - currentCount;
        Debug.LogFormat("{0} active hazards, want {1} at this time, adding {2} more", currentCount, targetCount, missing);

        while (m_freeSlots.Count > 0 && missing > 0)
        {
            var randomIndex = Random.Range(0, m_freeSlots.Count);
            var slot = m_freeSlots[randomIndex];
            m_freeSlots.RemoveAt(randomIndex);
            missing -= 1;

            var hazardIndex = Random.Range(0, m_spawnPrefabs.Length);
            var spawnAt = transform.GetChild(slot);

            var spawned = Instantiate(m_spawnPrefabs[hazardIndex], spawnAt.position, spawnAt.rotation, spawnAt);
            m_slots[slot] = spawned;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Application.isPlaying && i < m_slots.Length)
            {
                Gizmos.color = Color.Lerp(m_slots[i] ? Color.green : Color.red, Color.clear, 0.5f);
            }
            Gizmos.DrawSphere(transform.GetChild(i).position, m_gizmoSize);
        }
    }
}