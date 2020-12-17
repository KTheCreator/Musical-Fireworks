using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class AudioTriggerPreProcessed : MonoBehaviour
{
    public AudioSource audioSource;
    public SongController songController;
    [SerializeField]
    private VisualEffect fireworkFX;
    int i = 1;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Number of Points: "+songController.audPP.spectralFluxSamples.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
}
