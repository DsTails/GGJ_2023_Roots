using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Melanchall.DryWetMidi.Interaction;


public class Lane : MonoBehaviour
{
    

    public Melanchall.DryWetMidi.MusicTheory.NoteName restriction;
    [SerializeField] GameObject _noteObject;
    List<TrackNote> _notes = new List<TrackNote>();
    public List<double> timeStamps;

    public float spawnY;
    public float tapY;
    public float despawnY
    {
        get
        {
            return tapY - (spawnY - tapY);
        }
    }

    int spawnIndex = 0;

    [HideInInspector]
    public bool laneFinished;
    [HideInInspector]
    public bool laneStarted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnIndex < timeStamps.Count)
        {
            if(TrackScroller.GetAudioSourceTime() >= timeStamps[spawnIndex] - 2)
            {
                var note = Instantiate(_noteObject, transform.position, Quaternion.identity);
                note.transform.SetParent(this.transform);
                _notes.Add(note.GetComponent<TrackNote>());
                note.GetComponent<TrackNote>().spawnedYPos = spawnY;
                note.GetComponent<TrackNote>().failedYPos = despawnY;
                note.GetComponent<Subject>().AddObserver(GetComponentInParent<IObserver>());
                spawnIndex++;
            }
        } else if(spawnIndex >= timeStamps.Count && !laneFinished && laneStarted)
        {
            laneFinished = true;
            //GetComponentInParent<TrackScroller>().CheckLaneTracks();
        }
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] _notesArray)
    {
        foreach(var note in _notesArray)
        {
            if(note.NoteName == restriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, TrackScroller.midiFile.GetTempoMap());
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }

        laneStarted = true;

    }

    public int GetTimeStampCount()
    {
        return timeStamps.Count;
    }
    
}
