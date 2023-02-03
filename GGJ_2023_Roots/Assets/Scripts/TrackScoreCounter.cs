using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScoreCounter : Subject, IObserver
{
    [SerializeField] Subject _trackSubject;

    public int currentScore;
    public int currentMultiplier;

    public int scorePerNote = 100;
    public int scorePerGoodNote = 150;
    public int scorePerPerfectNote = 200;

    private void OnEnable()
    {
        _trackSubject.AddObserver(this);
    }

    private void OnDisable()
    {
        _trackSubject.RemoveObserver(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNotify(NoteEnum noteData)
    {
        ChangeScore(noteData);
    }

    public void ChangeScore(NoteEnum noteData)
    {
        switch (noteData)
        {
            case NoteEnum.normal:
                currentScore += scorePerNote;
                break;
            case NoteEnum.good:
                currentScore += scorePerGoodNote;
                break;
            case NoteEnum.perfect:
                currentScore += scorePerPerfectNote;
                break;
            case NoteEnum.miss:
                break;
            default:
                Debug.Log("NO VALID NOTE DATA CAN BE FOUND");
                break;
        }
    }

    void NoteHit()
    {

    }

    void NoteMiss()
    {

    }
}
