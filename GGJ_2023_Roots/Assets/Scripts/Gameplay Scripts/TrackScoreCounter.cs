using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class TrackScoreCounter : Subject, IObserver
{
    [SerializeField] Subject _trackSubject;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _audienceMoodText;
    [SerializeField] TextMeshProUGUI _bandMoodText;

    public AudioSource _as;
    public Animator crowdAnimator;
    public AudioClip crowdHappy;
    public AudioClip crowdSad;
    bool happySet;

    public int currentScore;
    public float currentMultiplier;

    public float bandMood;
    public float audienceMood;

    public int scorePerNote = 100;
    public int scorePerGoodNote = 150;
    public int scorePerPerfectNote = 200;

    public int[] multiplierThresholds;
    public float[] multipliers;
    int currentMultIndex = 0;
    public int thresholdTracker;
    public int comboNum;

    public float soloInterval;
    float _soloIntervalTime;

    bool _songActive;
    bool _soloActive;
    int _normalHits;
    int _goodHits;
    int _perfectHits;
    int _missedHits;

    int totalNoteCount;

    [SerializeField] Lane[] _lanes;

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
        _comboText.text = $"Combo: {comboNum}";
        _scoreText.text = $"Score: {currentScore}";
        _soloIntervalTime = soloInterval;

        bandMood = 75.0f;
        audienceMood = 65.0f;
        _bandMoodText.text = $"Band Mood: {bandMood.ToString("F1")}%";
        _audienceMoodText.text = $"Audience Mood: {bandMood.ToString("F1")}%";

        _soloIntervalTime = soloInterval;


        CheckMoodValues();

        _as.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (_songActive)
        {
            if (_soloIntervalTime > 0)
            {
                _soloIntervalTime -= 1 * Time.deltaTime;
            }
            else
            {
                if (_soloActive)
                {
                    UpdateSoloValues(-3f, 5f);
                }
                else
                {
                    UpdateSoloValues(5f, -3f);
                }

                _soloIntervalTime = soloInterval;
            }
        }
    }

    void UpdateSoloValues(float bandValChange, float audienceValChange)
    {
        bandMood += bandValChange;
        audienceMood += audienceValChange;

        if(bandMood <= 0.0f)
        {
            bandMood = 0.0f;
        } else if(bandMood >= 100.0f)
        {
            bandMood = 100.0f;
        }

        if(audienceMood <= 0.0f)
        {
            audienceMood = 0.0f;
        } else if(audienceMood >= 100.0f)
        {
            audienceMood = 100.0f;
        }

        _bandMoodText.text = $"Band Mood: {bandMood.ToString("F1")}%";
        _audienceMoodText.text = $"Audience Mood: {audienceMood.ToString("F1")}%";

        CheckMoodValues();
        

    }

    void CheckMoodValues()
    {
        if(bandMood <= 0.0f || audienceMood <= 0.0f)
        {
            //Game Over
            _songActive = false;

            if(bandMood <= 0.0f)
            {
                SongScoreData.lostAudience = false;
            }
            else
            {
                SongScoreData.lostAudience = true;
            }
            StartCoroutine(LoadGameOverCo());

        }

        
        if(audienceMood >= 50.0f && !happySet)
        {
            happySet = true;
            _as.clip = crowdHappy;
            _as.Play();
            crowdAnimator.Play("Happy");
        } else if(audienceMood < 50.0f && happySet)
        {
            happySet = false;
            _as.clip = crowdSad;
            _as.Play();
            crowdAnimator.Play("Sad");
        }
    }

    public void SoloTrigger(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _soloActive = true;
        }
        else if (context.canceled)
        {
            _soloActive = false;
        }
    }

    public void OnNotify(NoteEnum noteData)
    {
        if (noteData.ToString().Contains("Hit") || noteData.ToString().Contains("note"))
        {
            ChangeScore(noteData);
        }
        else if(noteData.ToString().Contains("End"))
        {
            _songActive = false;
           
            SongScoreData.normalHits = _normalHits;
            SongScoreData.goodHits = _goodHits;
            SongScoreData.perfectHits = _perfectHits;
            SongScoreData.missedHits = _missedHits;
            SongScoreData.finalScore = currentScore;

            StartCoroutine(LoadResultsCo());

        } else if(noteData == NoteEnum.songStart)
        {
            _songActive = true;
            foreach (var lane in _lanes) totalNoteCount += lane.GetTimeStampCount();
        }
    }

    public void ChangeScore(NoteEnum noteData)
    {
        switch (noteData)
        {
            case NoteEnum.normalHit:
                currentScore += (int)(scorePerNote * currentMultiplier);
                _normalHits++;
                NoteHit();
                break;
            case NoteEnum.goodHit:
                currentScore += (int)(scorePerGoodNote * currentMultiplier);
                _goodHits++;
                NoteHit();
                break;
            case NoteEnum.perfectHit:
                _perfectHits++;
                currentScore += (int)(scorePerPerfectNote * currentMultiplier);
                NoteHit();
                break;
            case NoteEnum.noteMiss:
                NoteMiss();
                break;
            default:
                _missedHits++;
                //Debug.Log("NO VALID NOTE DATA CAN BE FOUND");
                break;
        }
    }

    void NoteHit()
    {
        comboNum++;

        _scoreText.text = $"Score: {currentScore}";
        _comboText.text = $"Combo: {comboNum}";


        if(currentMultIndex < multiplierThresholds.Length - 1)
        {
            

            if(multiplierThresholds[currentMultIndex] <= comboNum)
            {
                currentMultIndex++;
                currentMultiplier = multipliers[currentMultIndex];
                thresholdTracker = 0;
            }
        }
    }
    void NoteMiss()
    {
        comboNum = 0;
        currentMultiplier = 1;
        _comboText.text = $"Combo: {comboNum}";
        _missedHits++;
    }

    IEnumerator LoadGameOverCo()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene("GameOver");
    }

    IEnumerator LoadResultsCo()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene("ResultScreen");
    }
}
