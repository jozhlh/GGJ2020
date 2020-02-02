using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField]
    private float m_holdToStartDuration = 2.0f;

    [SerializeField]
    private Color[] m_colors = new Color[4];
    
    [SerializeField]
    private GameObject[] m_heads = new GameObject[4];

    [Header("References")]
    [SerializeField]
    private MultiplayerCamera m_camera = null;

    [SerializeField]
    private LobbyUi m_lobbyUi = null;

    [SerializeField]
    private Hud m_hudUi = null;

    [SerializeField]
    private Transform[] m_spawnPositions = new Transform[4];

    private int[] m_headIndices = { 0, 0, 0, 0};

    private readonly List<Cosmonaut> m_activePlayers = new List<Cosmonaut>();


    public int ActivePlayerCount => m_activePlayers.Count;

    private float m_startCount = 0.0f;

    private bool m_inLobby = false;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.InGame = false;
        m_inLobby = true;
        m_activePlayers.Clear();

        GameEvents.OnPlayerDied += OnPlayerDied;
    }

    void OnDestroy()
    {
        GameEvents.OnPlayerDied -= OnPlayerDied;
    }

    void Update()
    {
        if (m_inLobby)
        {
            PrepareToStart();
        }
        else
        {
            m_camera.UpdateCamera();
        }
    }

    private void PrepareToStart()
    {
        var countup = false;
        foreach( var player in m_activePlayers)
        {
            if ( player.PlayerController.Start )
            {
                countup = true;
            }
        }

        CheckHeadChange();

        // if ( countup )
        // {
        //     GameEvents.InGame = true;
        //     GameEvents.OnRoundStart();
        //     Debug.Log("Round Start");
        //     m_inLobby = false;
        //     m_camera.GivePlayers(m_activePlayers);
        //     m_hudUi.CreatePlayerArrows(m_activePlayers, m_colors);
        // }

        if ( countup )
        {
            m_startCount += Time.deltaTime;
        }
        else
        {
            m_startCount -= ( 2.0f * Time.deltaTime );
        }

        m_startCount = Mathf.Max( m_startCount, 0.0f );

        m_lobbyUi.UpdateProgressBar( m_startCount / m_holdToStartDuration );

        if (m_startCount > m_holdToStartDuration)
        {
            SetCharacterHeads();
            GameEvents.InGame = true;
            GameEvents.OnRoundStart();
            Debug.Log("Round Start");
            m_inLobby = false;
            m_camera.GivePlayers(m_activePlayers);
            m_hudUi.CreatePlayerArrows(m_activePlayers, m_colors);
        }
    }

    private void SetCharacterHeads()
    {
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            var player = m_activePlayers[i];
            player.SetupVisuals( m_heads[m_headIndices[i]], m_colors[i] );
        }
    }


    private void CheckHeadChange()
    {
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            var player = m_activePlayers[i].PlayerController;

            if (player.Left)
            {
                // if player input ui left
                m_lobbyUi.ChangeHead( i, GetDifferentHead( i, -1 ));
            }
            
            if (player.Right)
            {
                // if player input ui right
                m_lobbyUi.ChangeHead( i, GetDifferentHead( i, 1 ));
            }

            player.ClearUiInput();
        }
    }


    private GameObject GetDifferentHead( int index, int direction )
    {
        var headIndex = m_headIndices[index];

        headIndex += direction;

        if (headIndex < 0)
        {
            headIndex = m_heads.Length - 1;
        }

        if (headIndex >= m_heads.Length)
        {
            headIndex = 0;
        }

        m_headIndices[index] = headIndex;

        return m_heads[headIndex];
    }


    private void OnPlayerJoined( PlayerInput player )
    {
        var cosmonaut = player.GetComponent<Cosmonaut>();
        var index = m_activePlayers.Count;
        m_activePlayers.Add( cosmonaut );

        cosmonaut.PlayerIndex = index;
        player.name = $"Cosmonaut_{index}";

        SpawnPlayer( player.transform, index );

        m_lobbyUi.PlayerJoined(index, m_colors[index]);
    }


    private void SpawnPlayer( Transform player, int index )
    {
        player.transform.position = m_spawnPositions[index].position;
        player.transform.rotation = Quaternion.identity;
        player.transform.parent = transform;
    }


    private void OnPlayerDied(Cosmonaut player)
    {
        SpawnPlayer(player.transform, player.PlayerIndex);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach( var spawn in m_spawnPositions )
        {
            Gizmos.DrawSphere( spawn.position, 0.2f );
        }
    }

    private void OnPlayerDied()
    {

    }
}
