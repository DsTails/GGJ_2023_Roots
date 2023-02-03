using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteButton : MonoBehaviour
{
    public Material defaultMaterial;
    public Material pressedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonCall(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GetComponent<Renderer>().material = pressedMaterial;
        } else if (context.canceled)
        {
            GetComponent<Renderer>().material = defaultMaterial;
        }
    }
}
