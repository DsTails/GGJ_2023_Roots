using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TrackNote : Subject
{
    public bool pressReady;
    bool _noteHit;

    NoteEnum noteData;

    double spawnedTime;

    public float spawnedYPos;
    public float failedYPos;

    public AudioClip missClip;

    [SerializeField] GameObject missUI;
    [SerializeField] GameObject okUI;
    [SerializeField] GameObject niceUI;
    [SerializeField] GameObject perfectUI;

    AudioSource _as;

    // Start is called before the first frame update
    void Start()
    {
        spawnedTime = TrackScroller.GetAudioSourceTime();
        _as = GetComponent<AudioSource>();
        Invoke(nameof(EnableMesh), .2f);
    }

    // Update is called once per frame
    void Update()
    {
        double lifeTime = TrackScroller.GetAudioSourceTime() - spawnedTime;

        float t = (float)(lifeTime / (1.45 * 2));

        

        if(t > 1)
        {
            //NotifyObservers(NoteEnum.noteDestroyed);
        }
        else
        {
            
            transform.localPosition = Vector3.Lerp(Vector3.up * spawnedYPos, Vector3.up * failedYPos, t);
        }
    }

    public void TriggerNote(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (pressReady)
            {
                _noteHit = true;
                //Debug.Log("NOTE HIT");

                

                Destroy(this.gameObject);

                if (Mathf.Abs(transform.position.y) > .25f)
                {
                    Debug.Log("Hit!");
                    noteData = NoteEnum.normal;
                    //Ok
                } else if(Mathf.Abs(transform.position.y) > 0.05)
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

            }
        }
    }

    void EnableMesh()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

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
                //Debug.Log("YOU MISSED A NOTE");
                noteData = NoteEnum.miss;
                NotifyObservers(noteData);
                _as.clip = missClip;
                _as.Play();
            }
        }
    }
}
