using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSelectPanel : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI m_joinUi = null;

    [SerializeField]
    private GameObject m_startUi = null;

    [SerializeField]
    private UiCharacter m_uiCharacter = null;

    [SerializeField]
    private Image[] m_playerColorImages = new Image[4];

    public void PlayerJoined( Color color )
    {
        m_joinUi.enabled = false;
        m_startUi.SetActive(true);
        m_uiCharacter.Join();

        foreach (var image in m_playerColorImages )
        {
            image.color = color;
        }
    }

    public void ChangeHead( GameObject head )
    {
        m_uiCharacter.ChangeHead( head );
    }
}
