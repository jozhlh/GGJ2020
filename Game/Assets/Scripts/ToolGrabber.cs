using System.Collections.Generic;
using UnityEngine;

public class ToolGrabber : GravitySource
{
    private float grabStrength;

    private readonly List<Tool> grabbableTools = new List<Tool>();
    public IReadOnlyList<Tool> GrabbableTools => grabbableTools;

    protected override void Awake()
    {
        base.Awake();

        grabStrength = Strength;
        Strength = 0;
    }

    public void BeginGrabbing()
    {
        Strength = grabStrength;
    }

    public void EndGrabbing()
    {
        Strength = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<Tool>() is Tool tool)
        {
            this.grabbableTools.Add(tool);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponentInParent<Tool>() is Tool tool)
        {
            this.grabbableTools.Remove(tool);
        }
    }
}