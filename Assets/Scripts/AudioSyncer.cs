using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
    public float bias; // Determines the threshold of the beat.
    public float timeStep, timeToBeat, restSmoothTime;
    [SerializeField]
    private float previousAudioValue, currentAudioValue, timer;
    [SerializeField]
    protected bool isBeat;
    
    public virtual void OnBeat()
    {
        timer = 0;
        isBeat = true;
    }
    public virtual void OnUpdate()
    {
        previousAudioValue = currentAudioValue;
        currentAudioValue = AudioSpectrum.spectrumValue;
        if(previousAudioValue > bias && currentAudioValue <= bias)
        {
            if(timer > timeStep)
            {
                OnBeat();
            }
        }
        if(previousAudioValue <= bias && currentAudioValue > bias)
        {
            OnBeat();
        }
        timer += Time.deltaTime;
    }
    // Update is called once per frame
    private void Update()
    {
        OnUpdate();
    }
}
