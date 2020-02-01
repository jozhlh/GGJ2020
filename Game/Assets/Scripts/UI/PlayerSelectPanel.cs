using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelectPanel : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI m_joinUi = null;

    [SerializeField]
    private TextMeshProUGUI m_startUi = null;

    public void PlayerJoined()
    {
        m_joinUi.enabled = false;
        m_startUi.enabled = true;
    }
}
