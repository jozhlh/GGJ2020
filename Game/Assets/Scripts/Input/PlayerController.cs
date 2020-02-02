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

    private bool m_left = false;

    private bool m_right = false;

    public Vector2 Move => m_move;
    public Vector2 Aim => m_aim;

    public bool Grab => m_grab;

    public bool Jump => m_jump;

    public bool Interact => m_interact;

    public bool Start => m_start;

    public bool Left => m_left;

    public bool Right => m_right;
    

    public void ClearUiInput()
    {
        m_left = false;
        m_right = false;
    }


    // Call from Fixedupdates to ensure inputs are not dropped between fixed frame updates
    public void UpdateInput()
    {
        m_jump = m_cachedJump;
        m_interact |= m_cachedInteract;
        m_start |= m_cachedStart;
        m_grab |= m_cachedGrab;
        
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
        Debug.Log("On Jump Down");
    }

    // Input system callback event
    private void OnInteractDown()
    {
        m_interact = true;
        m_cachedInteract = true;
        Debug.Log("On Interact Down");
    }

    private void OnGrabDown()
    {
        m_cachedGrab = true;
        m_grab = true;
        Debug.Log($"On Grab Down");
    }

    // Input system callback event
    private void OnStartDown()
    {
        m_cachedStart = true;
        m_start = true;
        Debug.Log("On Start Down");
    }

    // Input system callback event
    private void OnInteractUp()
    {
        m_interact = false;
        Debug.Log("On Interact Up");
    }

    private void OnGrabUp()
    {
        m_grab = false;
        Debug.Log($"On Grab Up");
    }

    // Input system callback event
    private void OnStartUp()
    {
        m_start = false;
        Debug.Log("On Start Up");
    }

    private void OnLeft()
    {
        m_left = true;
    }


    private void OnRight()
    {
        m_right = true;
    }
}
