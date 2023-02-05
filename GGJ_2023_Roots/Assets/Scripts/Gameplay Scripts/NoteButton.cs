using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteButton : MonoBehaviour
{
    public Material defaultMaterial;
    public Material pressedMaterial;
    public Material greyedOutMat;

    bool _soloActive;

    Collider _collider;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonCall(InputAction.CallbackContext context)
    {
        if (!_soloActive)
        {
            if (context.started)
            {
                GetComponent<Renderer>().material = pressedMaterial;
            }
            else if (context.canceled)
            {
                GetComponent<Renderer>().material = defaultMaterial;
            }
        }
    }

    public void TriggerSolo(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _collider.enabled = false;
            GetComponent<Renderer>().material = greyedOutMat;
            _soloActive = true;
        } else if (context.canceled)
        {
            _collider.enabled = true;
            GetComponent<Renderer>().material = defaultMaterial;
            _soloActive = false;
        }
    }
}
