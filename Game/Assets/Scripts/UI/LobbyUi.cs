﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUi : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerSelectPanel[] m_playerSelectPanels = new PlayerSelectPanel[4];

    [SerializeField]
    private Image m_startProgress = null;

    [SerializeField]
    private Canvas m_lobbyCanvas = null;

    private void Start()
    {
        GameEvents.OnRoundStart += StartRound;
    }

    private void OnDestroy()
    {
        GameEvents.OnRoundStart -= StartRound;
    }

    public void PlayerJoined( int index, Color color )
    {
        m_playerSelectPanels[index].PlayerJoined( color );
    }

    public void ChangeHead( int index, GameObject head, Color color )
    {
        m_playerSelectPanels[index].ChangeHead( head, color );
    }

    public void UpdateProgressBar( float t )
    {
        m_startProgress.fillAmount = t;
    }

    private void StartRound()
    {
        m_lobbyCanvas.enabled = false;
    }
}
