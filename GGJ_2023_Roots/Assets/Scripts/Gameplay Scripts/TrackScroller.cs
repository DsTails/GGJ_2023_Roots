using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
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

    public string fileLocation;

    public static MidiFile midiFile;

    [SerializeField] Lane[] _lanes;

    public static AudioSource _as;

    public AudioSource bassAs;

    public AudioClip bassNormalClip;
    public AudioClip bassSoloClip;

    public float songDelay;

    [SerializeField] TextMeshProUGUI _countdownText;

    int noteCountTotal;
    int noteCount;

    bool _songActive;

    // Start is called before the first frame update
    void Start()
    {
        _bps = bpm / 60;


        _as = GetComponent<AudioSource>();

        if (SongSelectData.loadedSong != null)
        {
            _as.clip = SongSelectData.loadedSong;
            bassAs.clip = SongSelectData.loadedNormalBass;
            bassNormalClip = SongSelectData.loadedNormalBass;
            bassSoloClip = SongSelectData.loadedSoloBass;
            fileLocation = SongSelectData.tempoMapLocation;
        }

        //ReadFromFile();

        StartCoroutine(StartSongCo());
    }

    // Update is called once per frame
    void Update()
    {
        if(_songActive && !_as.isPlaying)
        {
            _songActive = false;
            bassAs.Stop();
            StartCoroutine(EndSongCo());
        }
    }

    public static double GetAudioSourceTime()
    {
        return (double)_as.timeSamples / _as.clip.frequency;
    }

    public void OnNotify(NoteEnum noteData)
    {
        /*
        if (noteData != NoteEnum.songEnd && noteData != NoteEnum.noteDestroyed && noteData != NoteEnum.songStart)
        {
            //Potentially call
            //Debug.Log($"NOTE DATA: {noteData.ToString()}");

            NotifyObservers(noteData);

            noteCount++;

            CheckNoteCount();
        }
        else if(noteData == NoteEnum.songEnd)
        {
            Debug.Log("SONG ENDED");
            _as.Stop();
        }
        else
        {
            noteCount++;
            CheckNoteCount();
        }*/

        if (noteData.ToString().Contains("Hit") || noteData.ToString().Contains("note"))
        {
            NotifyObservers(noteData);
            //noteCount++;
            //CheckNoteCount();
        }
        else if (noteData.ToString().Contains("End"))
        {
            _as.Stop();
        }
    }

    public void TriggerSolo(InputAction.CallbackContext context)
    {
        if (_songActive)
        {
            if (context.started)
            {
                bassAs.clip = bassSoloClip;
                bassAs.Play();
                bassAs.time = _as.time;

            }
            else if (context.canceled)
            {
                bassAs.clip = bassNormalClip;
                bassAs.Play();
                bassAs.time = _as.time;
            }
        }
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
            Debug.Log("ENDING SONG");
            //NotifyObservers(NoteEnum.songEnd);

            StartCoroutine(EndSongCo());
        }
        else
        {
            Debug.Log(noteCount);
        }
    }

    public void RestartSong()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToSongMenu()
    {
        SceneManager.LoadScene("SongSelect");
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

        //Debug.Log(notesArray.Length);

        foreach (var lane in _lanes) lane.SetTimeStamps(notesArray);

        foreach (var lane in _lanes) noteCountTotal += lane.GetTimeStampCount();

        Debug.Log(noteCountTotal);

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
        
        NotifyObservers(NoteEnum.songStart);
        _as.Play();
        bassAs.Play();
        _songActive = true;
    }

    IEnumerator EndSongCo()
    {
        yield return new WaitForSeconds(.5f);
        NotifyObservers(NoteEnum.songEnd);
    }
}
