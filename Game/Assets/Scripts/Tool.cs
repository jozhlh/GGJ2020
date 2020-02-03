using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract void BeginUsing(Cosmonaut user);
    public abstract void StopUsing(Cosmonaut user);
    public abstract bool IsUsing { get; }

    [SerializeField]
    private Collider m_collider;

    [SerializeField]
    private FloatingBody floatingBody;
    public FloatingBody FloatingBody => floatingBody;

    public virtual void Grab(Cosmonaut holder)
    {
        floatingBody.Rigidbody.isKinematic = true;
        m_collider.enabled = false;

        transform.SetParent( holder.HandPosition, false);
        transform.position = holder.HandPosition.transform.position;
        //transform.rotation = holder.HandPosition.transform.rotation;

        transform.right = (transform.position - holder.transform.position).normalized;

        //floatingBody.enabled = false;
        
    }

    public void UnGrab(Cosmonaut holder)
    {
        if (IsUsing)
        {
            StopUsing(holder);
        }

        transform.SetParent(holder.transform.parent, true);

        //floatingBody.enabled = true;
        floatingBody.Rigidbody.isKinematic = false;
        m_collider.enabled = true;
    }
}