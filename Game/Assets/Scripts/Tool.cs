using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public abstract void BeginUsing(Cosmonaut user);
    public abstract void StopUsing(Cosmonaut user);
    public abstract bool IsUsing { get; }

    [SerializeField]
    private FloatingBody floatingBody;
    public FloatingBody FloatingBody => floatingBody;

    public void Grab(Cosmonaut holder)
    {
        transform.SetParent( holder.HandPosition, false);
        transform.position = holder.HandPosition.transform.position;
        transform.rotation = holder.HandPosition.transform.rotation;

        floatingBody.enabled = false;
    }

    public void UnGrab(Cosmonaut holder)
    {
        if (IsUsing)
        {
            StopUsing(holder);
        }

        transform.SetParent(holder.transform.parent, true);

        floatingBody.enabled = true;
    }
}