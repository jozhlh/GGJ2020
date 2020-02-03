using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCamera : MonoBehaviour
{
    [SerializeField]
    private float m_minFov = 40.0f;

    [SerializeField]
    private float m_maxFov = 100.0f;

    [SerializeField]
    private float m_fovLerp = 0.1f;

    [SerializeField]
    private float m_moveLerp = 0.1f;

    [SerializeField]
    private float m_yOffset = 2.0f;

    [SerializeField]
    private float m_fovScale = 0.3f;

    [SerializeField]
    private Camera m_camera = null;

    private readonly List<Transform> m_players = new List<Transform>();

    private Vector3 m_centreOfMass = Vector3.zero;

    private Vector2 m_extents = Vector2.zero;

    private float m_targetFov = 0.0f;


    // Update is called once per frame
    public void UpdateCamera()
    {
        GetValues();
        MoveCamera();
        CalculateFov();
    }

    public void GivePlayers( List<Cosmonaut> cosmonauts )
    {
        m_players.Clear();
        foreach( var player in cosmonauts)
        {
            m_players.Add(player.GetComponentInChildren<FloatingBody>().transform);
        }
    }

    private void GetValues()
    {
        if (m_players.Count < 1)
        {
            return;
        }

        m_centreOfMass = Vector3.zero;
        var bot = m_players[0].position;
        var top = m_players[0].position;

        foreach( var player in m_players)
        {
            var pos = player.position;
            m_centreOfMass += pos;
            bot.x = Mathf.Min(bot.x, pos.x);
            bot.y = Mathf.Min(bot.y, pos.y);
            top.x = Mathf.Max(top.x, pos.x);
            top.y = Mathf.Max(top.y, pos.y);
        }

        m_extents.x = Mathf.Abs(top.x - bot.x);
        m_extents.y = Mathf.Abs(top.y - bot.y);

        m_centreOfMass /= (float)m_players.Count;
    }

    private void MoveCamera()
    {
        var pos = transform.position;
        pos.x = m_centreOfMass.x;
        pos.y = m_centreOfMass.y + m_yOffset;

        transform.position = Vector3.Lerp( transform.position, pos, m_moveLerp );
    }

    private void CalculateFov()
    {
        var ratio = 1.0f;
        var x = m_extents.x * 0.5f;
        var y = m_extents.y * 0.5f;

        var xRatio = x / 16.0f;
        var yRatio = y / 9.0f;

        var z = Mathf.Abs(transform.position.z);

        var fov = 0.0f;

        if (xRatio > yRatio)
        {
            fov = Mathf.Atan(x / z) * Mathf.Rad2Deg;
        }
        else
        {
            y = (y / 9.0f) * 16.0f;
            fov = Mathf.Atan(y / z) * Mathf.Rad2Deg;
            ratio = (ratio / 9.0f) * 16.0f;
        }

        fov *= m_fovScale; 

        if (fov < m_minFov)
        {
            fov = m_minFov;
        }

        if (fov > m_maxFov)
        {
            fov = m_maxFov;
        }

        m_targetFov = fov;

        m_camera.fieldOfView = Mathf.Lerp( m_camera.fieldOfView, m_targetFov, m_fovLerp );
    }


}
