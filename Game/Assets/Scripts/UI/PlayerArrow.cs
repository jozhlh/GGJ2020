using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float m_radius = 500.0f;

    [SerializeField]
    private Sprite[] m_playerSprites = new Sprite[4];

    [Header("References")]
    [SerializeField]
    private RectTransform m_transform = null;

    [SerializeField]
    private CanvasGroup m_group = null;

    [SerializeField]
    private Image m_image = null;

    [SerializeField]
    private Image m_arrow = null;

    private Transform m_player = null;

    private Camera m_camera = null;

    private RectTransform m_canvas = null;    


    // Update is called once per frame
    void Update()
    {
        GetPointOnCanvas();
    }


    public void Setup( Camera mainCamera, Transform player, Canvas canvas, int index, Color color )
    {
        m_camera = mainCamera;
        m_player = player;
        m_canvas = canvas.GetComponent<RectTransform>();
        m_image.sprite = m_playerSprites[index];
        m_image.color = color;
        m_arrow.color = color;
    }


    private void GetPointOnCanvas()
    {      
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        var viewportPosition = m_camera.WorldToViewportPoint( m_player.position );

        var screenPosition = new Vector2(((viewportPosition.x*m_canvas.sizeDelta.x)-(m_canvas.sizeDelta.x*0.5f)), ((viewportPosition.y*m_canvas.sizeDelta.y)-(m_canvas.sizeDelta.y*0.5f)));

        if ((Mathf.Abs(screenPosition.x) > (m_canvas.sizeDelta.x * 0.5f)) || (Mathf.Abs(screenPosition.y) > (m_canvas.sizeDelta.y*0.5f)))
        {
            m_group.alpha = 1.0f;
        }
        else
        {
            m_group.alpha = 0.0f;
        }

        screenPosition = screenPosition.normalized * m_radius;
        
        //now you can set the position of the ui element
        m_transform.anchoredPosition = screenPosition;

        var angle = Vector2.SignedAngle( Vector2.up, screenPosition);

        m_transform.rotation = Quaternion.Euler( 0.0f, 0.0f, angle);
    }
}
