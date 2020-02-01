using System.Drawing;
using UnityEngine;
using System;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private float m_gizmoSize;

    [Space(10)]
    [SerializeField] private float m_missionDurationMinutes;
    [SerializeField] private float m_missionDurationSeconds;

    [Space(10)]
    [SerializeField] private GameObject[] m_hazardPrefabs;

    [Space(10)]
    [SerializeField] private AnimationCurve m_chanceOfSpawnOverTime;
    [SerializeField] private float m_spawnRatePerSecond;

    private float m_startTime = 0f;
    private float m_totalMissionDurationSeconds;

    private void Start()
    {
        m_startTime = Time.time;
        m_totalMissionDurationSeconds = m_missionDurationMinutes * 60f + m_missionDurationSeconds;
    }
    private void Update()
    {
        var tnsfm = transform;
        int spawnPoints = tnsfm.childCount;
        if (Time.frameCount % 60 == 0 && m_hazardPrefabs.Length > 0 && spawnPoints > 0)
        {
            float normalisedTime = Mathf.Clamp01((Time.time - m_startTime) / m_totalMissionDurationSeconds);
            float chance = m_chanceOfSpawnOverTime.Evaluate(normalisedTime) * m_spawnRatePerSecond;
            if (Random.Range(0f, 1f) < chance)
            {
                int spawnerIndex = Random.Range(0, m_hazardPrefabs.Length);
                GameObject hazard = m_hazardPrefabs[spawnerIndex];
                
                int hazardPosIndex = Random.Range(0, spawnPoints);
                Vector3 hazardPos = tnsfm.GetChild(hazardPosIndex).position;

                Instantiate(hazard, hazardPos, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, m_gizmoSize);
        }
    }
}