using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [SerializeField]
    private Camera m_camera = null;

    [SerializeField]
    private Canvas m_hudCanvas = null;

    [SerializeField]
    private Canvas m_parentCanvas = null;

    [SerializeField]
    private PlayerArrow m_playerArrowPrefab = null;

    public void CreatePlayerArrows( List<Cosmonaut> cosmonauts, Color[] colors )
    {
        m_hudCanvas.enabled = true;
        int index = 0;

        if (!m_playerArrowPrefab)
        {
            return;
        }
        foreach( var cosmonaut in cosmonauts )
        {
            var arrow = Instantiate( m_playerArrowPrefab, transform, false);
            arrow.Setup( m_camera, cosmonaut.GetComponentInChildren<FloatingBody>().transform, m_parentCanvas, index, colors[index] );
            index++;
        }
    }
}
