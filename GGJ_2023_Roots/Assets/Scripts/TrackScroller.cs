using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackScroller : Subject, IObserver
{
    public float bpm;
    //beats per second. Divide bpm by 60 (a minute)
    float _bps;
    public bool hasStarted;

    [SerializeField] GameObject[] _noteTracks;

    GameObject currentNoteTrack;

    

    // Start is called before the first frame update
    void Start()
    {
        _bps = bpm / 60;

        List<Subject> noteSubjects = GetComponentsInChildren<Subject>().ToList();

        noteSubjects.Remove(this);

        

        noteSubjects.ForEach((_subject) =>
        {
            _subject.AddObserver(this);
            
        });

        GetObserverNames();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            transform.position -= new Vector3(0f, _bps * Time.deltaTime, 0f);
        }
    }

    public void OnNotify(NoteEnum noteData)
    {
        //Potentially call
        //Debug.Log($"NOTE DATA: {noteData.ToString()}");

        NotifyObservers(noteData);
    }
}
