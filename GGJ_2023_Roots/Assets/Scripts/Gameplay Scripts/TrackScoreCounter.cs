using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackScoreCounter : Subject, IObserver
{
    [SerializeField] Subject _trackSubject;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _comboText;

    [SerializeField] GameObject _resultsScreen;
    [SerializeField] TextMeshProUGUI _normalHitsText;
    [SerializeField] TextMeshProUGUI _goodHitsText;
    [SerializeField] TextMeshProUGUI _perfectHitsText;
    [SerializeField] TextMeshProUGUI _missHitsText;
    [SerializeField] TextMeshProUGUI _percentHitText;
    [SerializeField] TextMeshProUGUI _rankText;
    [SerializeField] TextMeshProUGUI _finalScoreText;

    public int currentScore;
    public float currentMultiplier;

    public int scorePerNote = 100;
    public int scorePerGoodNote = 150;
    public int scorePerPerfectNote = 200;

    public int[] multiplierThresholds;
    public float[] multipliers;
    int currentMultIndex = 0;
    public int thresholdTracker;
    public int comboNum;


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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNotify(NoteEnum noteData)
    {
        if (noteData != NoteEnum.songEnd)
        {
            ChangeScore(noteData);
        }
        else
        {
            if (!_resultsScreen.activeInHierarchy)
            {
                _resultsScreen.SetActive(true);

                _normalHitsText.text = $"Normal Hits: {_normalHits}";
                _goodHitsText.text = $"Good Hits: {_goodHits}";
                _perfectHitsText.text = $"Perfect Hits: {_perfectHits}";
                _missHitsText.text = $"Misses Hits: {_missedHits}";

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
