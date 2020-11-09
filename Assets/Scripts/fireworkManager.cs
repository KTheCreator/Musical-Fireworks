using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class fireworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private VisualEffect fireWorkSystem;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fireWorkSystem.SendEvent("OnPlay");
        }
    }
}
