using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator DoorAnimator;
    private bool open=false;

    // Start is called before the first frame update
    void OnTriggerStay()
    {
        open=true;

    }

    // Update is called once per frame
    void OnTriggerExit()
    {
        open=false;

    }
    void Update()
    {
        DoorAnimator.SetInteger("DoorState",open?1:0);
    }
}
