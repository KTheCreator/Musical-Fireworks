using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AudioTrigger : AudioSyncer
{
    [SerializeField]
    private VisualEffect fireworkFX;
    // Start is called before the first frame update
    public override void OnBeat()
    {
        base.OnBeat();
        fire();
    }
    // Update is called once per frame
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isBeat) return;
        Debug.Log("Resting");
    }

    private void fire()
    {
        Debug.Log("fireMe");
        fireworkFX.SendEvent("FireMain");
        isBeat = false;
    }
}
