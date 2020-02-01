using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 m_move = Vector2.zero;

    private bool m_jump = false;

    private bool m_interact = false;

    private bool m_cachedJump = false;

    private bool m_cachedInteract = false;

    public Vector2 Move => m_move;

    public bool Jump => m_jump;

    public bool Interact => m_interact;

    // Call from Fixedupdates to ensure inputs are not dropped between fixed frame updates
    public void UpdateInput()
    {
        m_jump = m_cachedJump;
        m_interact = m_cachedInteract;

        m_cachedJump = false;
        m_cachedInteract = false;
    }

    // Input system callback event
    private void OnMove( InputValue value )
    {
        m_move = value.Get<Vector2>();
    }

    // Input system callback event
    private void OnJump()
    {
        m_cachedJump = true;
        Debug.Log("On Jump");
    }

    // Input system callback event
    private void OnInteract()
    {
        m_cachedInteract = true;
        Debug.Log("On Interact");
    }
}
