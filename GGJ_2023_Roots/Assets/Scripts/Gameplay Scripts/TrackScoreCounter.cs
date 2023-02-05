using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TrackScoreCounter : Subject, IObserver
{
    [SerializeField] Subject _trackSubject;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;
    [SerializeField] TextMeshProUGUI _audienceMoodText;
    [SerializeField] TextMeshProUGUI _bandMoodText;

    [SerializeField] GameObject _resultsScreen;
    [SerializeField] TextMeshProUGUI _normalHitsText;
    [SerializeField] TextMeshProUGUI _goodHitsText;
    [SerializeField] TextMeshProUGUI _perfectHitsText;
    [SerializeField] TextMeshProUGUI _missHitsText;
    [SerializeField] TextMeshProUGUI _percentHitText;
    [SerializeField] TextMeshProUGUI _rankText;
    [SerializeField] TextMeshProUGUI _finalScoreText;

    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] TextMeshProUGUI _gameOverText;

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

        bandMood = 50.0f;
        audienceMood = 50.0f;
        _bandMoodText.text = $"Band Mood: {bandMood.ToString("F1")}%";
        _audienceMoodText.text = $"Audience Mood: {bandMood.ToString("F1")}%";

        _soloIntervalTime = soloInterval;
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
                    UpdateSoloValues(-5f, 2.5f);
                }
                else
                {
                    UpdateSoloValues(2.5f, -5f);
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
            if (!_gameOverScreen.activeInHierarchy)
            {
                _gameOverScreen.SetActive(true);
                if(bandMood <= 0.0f)
                {
                    _gameOverText.text = "We told you to stick to the root notes! \n You're out!";
                }
                else
                {
                    _gameOverText.text = "The audience got bored and decided to leave...";
                }
            }

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
        if (noteData != NoteEnum.songEnd && noteData != NoteEnum.songStart)
        {
            ChangeScore(noteData);
        }
        else if(noteData == NoteEnum.songEnd)
        {
            _songActive = false;

            if (!_resultsScreen.activeInHierarchy)
            {
                _resultsScreen.SetActive(true);

                _normalHitsText.text = $"Normal Hits: {_normalHits}";
                _goodHitsText.text = $"Good Hits: {_goodHits}";
                _perfectHitsText.text = $"Perfect Hits: {_perfectHits}";
                _missHitsText.text = $"Misses Hits: {_missedHits}";
                _finalScoreText.text = $"Final Score: {currentScore}";

                int totalHits = _normalHits + _goodHits + _perfectHits + _missedHits;

                Debug.Log(totalHits);

                int totalHitCount = _normalHits + _goodHits + _perfectHits;

                Debug.Log(totalHitCount);

                float percentHits = ((float)totalHitCount / (float)totalHits) * 100.0f;

                Debug.Log(percentHits);

                _percentHitText.text = percentHits.ToString("F1") + "%";

                string rankVal = "F";

                if(percentHits > 40f)
                {
                    rankVal = "D";
                    if(percentHits > 55f)
                    {
                        rankVal = "C";
                        if(percentHits > 70f)
                        {
                            rankVal = "B";
                            if(percentHits > 85)
                            {
                                rankVal = "A";
                                if(percentHits > 95)
                                {
                                    rankVal = "S";
                                }
                            }
                        }
                    }
                }

                _rankText.text = rankVal;


            }
        } else if(noteData == NoteEnum.songStart)
        {
            _songActive = true;
        }
    }

    public void ChangeScore(NoteEnum noteData)
    {
        switch (noteData)
        {
            case NoteEnum.normal:
                currentScore += (int)(scorePerNote * currentMultiplier);
                _normalHits++;
                NoteHit();
                break;
            case NoteEnum.good:
                currentScore += (int)(scorePerGoodNote * currentMultiplier);
                _goodHits++;
                NoteHit();
                break;
            case NoteEnum.perfect:
                _perfectHits++;
                currentScore += (int)(scorePerPerfectNote * currentMultiplier);
                NoteHit();
                break;
            case NoteEnum.miss:
                NoteMiss();
                break;
            default:
                Debug.Log("NO VALID NOTE DATA CAN BE FOUND");
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
}
