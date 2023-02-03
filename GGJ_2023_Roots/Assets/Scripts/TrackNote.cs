using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TrackNote : Subject
{
    public bool pressReady;
    bool _noteHit;

    NoteEnum noteData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerNote(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (pressReady)
            {
                _noteHit = true;
                //Debug.Log("NOTE HIT");

                if(Mathf.Abs(transform.position.z) > .25f)
                {
                    Debug.Log("Hit!");
                    noteData = NoteEnum.normal;
                    //Ok
                } else if(Mathf.Abs(transform.position.z) > 0.05)
                {
                    //Good
                    Debug.Log("Jamming!");
                    noteData = NoteEnum.good;
                }
                else
                {
                    //Perfect
                    Debug.Log("Rock-tacular!");
                    noteData = NoteEnum.perfect;
                }

                
                NotifyObservers(noteData);

                Destroy(this.gameObject);
            }
        }
    }

    //

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "NoteButton")
        {
            
            pressReady = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "NoteButton")
        {
            pressReady = false;
            if (!_noteHit)
            {
                Debug.Log("YOU MISSED A NOTE");
                noteData = NoteEnum.miss;
                NotifyObservers(noteData);
            }
        }
    }
}
