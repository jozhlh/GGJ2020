using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerInput m_input = null;
    
    private Vector2 m_move = Vector2.zero;

    private Vector2 m_aim = Vector2.zero;

    private bool m_jump = false;

    private bool m_interact = false;

    private bool m_grab = false;

    private bool m_start = false;

    private bool m_cachedJump = false;

    private bool m_cachedInteract = false;

    private bool m_cachedGrab = false;

    private bool m_cachedStart = false;

    public Vector2 Move => m_move;
    public Vector2 Aim => m_aim;

    public bool Grab => m_grab;

    public bool Jump => m_jump;

    public bool Interact => m_interact;

    public bool Start => m_start;


    // Call from Fixedupdates to ensure inputs are not dropped between fixed frame updates
    public void UpdateInput()
    {
        m_jump = m_cachedJump;
        m_interact = m_cachedInteract;
        m_start = m_cachedStart;
        m_grab = m_cachedGrab;
        
        m_cachedJump = false;
        m_cachedInteract = false;
        m_cachedStart = false;
        m_cachedGrab = false;
    }

    // Input system callback event
    private void OnMove( InputValue value )
    {
        m_move = value.Get<Vector2>();
    }

    // Input system callback event
    private void OnAim( InputValue value )
    {
        m_aim = value.Get<Vector2>();
    }

    // Input system callback event
    private void OnJump()
    {
        m_cachedJump = true;
        Debug.Log("On Jump");
    }

    // Input system callback event
    private void OnInteract( InputValue value )
    {
        m_cachedInteract = value.Get<float>() > 0;
        Debug.Log("On Interact");
    }

    private void OnGrab( InputValue value )
    {
        m_cachedGrab = value.Get<float>() > 0;
        Debug.Log($"On Grab {m_cachedGrab}");
    }

    // Input system callback event
    private void OnStart()
    {
        m_cachedStart = true;
        Debug.Log("On Start");
    }
}
