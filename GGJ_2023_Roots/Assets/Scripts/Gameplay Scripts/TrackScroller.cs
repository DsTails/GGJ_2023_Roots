using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using TMPro;

public class TrackScroller : Subject, IObserver
{
    public float bpm;
    //beats per second. Divide bpm by 60 (a minute)
    float _bps;
    public bool hasStarted;

    public string fileLocation;

    public static MidiFile midiFile;

    [SerializeField] Lane[] _lanes;

    public static AudioSource _as;

    public float songDelay;

    [SerializeField] TextMeshProUGUI _countdownText;

    int noteCountTotal;
    int noteCount;

    // Start is called before the first frame update
    void Start()
    {
        _bps = bpm / 60;


        _as = GetComponent<AudioSource>();

        if (SongSelectData.loadedSong != null)
        {
            _as.clip = SongSelectData.loadedSong;
            fileLocation = SongSelectData.tempoMapLocation;
        }

        /*
        List<Subject> noteSubjects = GetComponentsInChildren<Subject>().ToList();

        noteSubjects.Remove(this);

        

        noteSubjects.ForEach((_subject) =>
        {
            _subject.AddObserver(this);
            
        });

        GetObserverNames();*/

        //ReadFromFile();

        StartCoroutine(StartSongCo());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            //transform.position -= new Vector3(0f, _bps * Time.deltaTime, 0f);
        }
    }

    public static double GetAudioSourceTime()
    {
        return (double)_as.timeSamples / _as.clip.frequency;
    }

    public void OnNotify(NoteEnum noteData)
    {
        //Potentially call
        //Debug.Log($"NOTE DATA: {noteData.ToString()}");

        NotifyObservers(noteData);

        noteCount++;

        CheckNoteCount();
    }

    public void CheckLaneTracks()
    {
        bool songFinished = true;

        foreach(var lane in _lanes)
        {
            if (!lane.laneFinished)
            {
                songFinished = false;
            }
        }

        if (songFinished)
        {
            //Display Score:
            
                
            NotifyObservers(NoteEnum.songEnd);
            
        }
    }

    public void CheckNoteCount()
    {
        if(noteCount >= noteCountTotal)
        {

            //NotifyObservers(NoteEnum.songEnd);

            StartCoroutine(EndSongCo());
        }
    }

    void ReadFromFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);

        GetDataFromMidiFile();
    }

    void GetDataFromMidiFile()
    {
        var notes = midiFile.GetNotes();
        var notesArray = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];

        notes.CopyTo(notesArray, 0);

        foreach (var lane in _lanes) lane.SetTimeStamps(notesArray);

        foreach (var lane in _lanes) noteCountTotal += lane.GetTimeStampCount();

        Invoke(nameof(StartSong), songDelay);

        //Count down here
    }

    IEnumerator StartSongCo()
    {
        float timerToStart = int.Parse(_countdownText.text);

        while(timerToStart > 0)
        {
            timerToStart -= 1 * Time.deltaTime;
            _countdownText.text = timerToStart.ToString("0");

            yield return null;
        }

        _countdownText.gameObject.SetActive(false);
        ReadFromFile();
    }

    void StartSong()
    {
        _as.Play();
    }

    IEnumerator EndSongCo()
    {
        yield return new WaitForSeconds(.5f);
        NotifyObservers(NoteEnum.songEnd);
    }
}
