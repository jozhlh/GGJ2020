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
        floatingBody.transform.SetParent( holder.HandPosition, false);
        floatingBody.transform.localPosition = Vector3.zero;
        floatingBody.transform.localRotation = Quaternion.identity;

        floatingBody.enabled = false;
    }

    public void UnGrab(Cosmonaut holder)
    {
        if (IsUsing)
        {
            StopUsing(holder);
        }

        floatingBody.transform.SetParent(holder.transform.parent, true);

        floatingBody.enabled = true;
    }
}