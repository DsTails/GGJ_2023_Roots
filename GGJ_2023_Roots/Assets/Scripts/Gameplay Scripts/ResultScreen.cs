using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _normalHitsText;
    [SerializeField] TextMeshProUGUI _goodHitsText;
    [SerializeField] TextMeshProUGUI _perfectHitsText;
    [SerializeField] TextMeshProUGUI _missHitsText;
    [SerializeField] TextMeshProUGUI _percentHitText;
    [SerializeField] TextMeshProUGUI _rankText;
    [SerializeField] TextMeshProUGUI _finalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SongScoreData.normalHits);
        Debug.Log(SongScoreData.goodHits);
        Debug.Log(SongScoreData.perfectHits);
        Debug.Log(SongScoreData.missedHits);
        Debug.Log(SongScoreData.finalScore);

        StartCoroutine(DisplayResultsCo());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitToSongSelect()
    {
        StartCoroutine(ResultDecisionCo(true));
    }

    public void RestartSong()
    {
        StartCoroutine(ResultDecisionCo(false));
    }

    IEnumerator DisplayResultsCo()
    {
        _normalHitsText.text = $"Ok! Hits: {SongScoreData.normalHits}";
        yield return new WaitForSeconds(.3f);
        _goodHitsText.text = $"Nice! Hits: {SongScoreData.goodHits}";
        yield return new WaitForSeconds(.3f);
        _perfectHitsText.text = $"Perfect! Hits: {SongScoreData.perfectHits}";
        yield return new WaitForSeconds(.3f);
        _missHitsText.text = $"Missed! Hits: {SongScoreData.missedHits}";
        yield return new WaitForSeconds(.3f);
        int totalHits = SongScoreData.normalHits + SongScoreData.goodHits + SongScoreData.perfectHits + SongScoreData.missedHits;
        int totalHitsMade = SongScoreData.normalHits + SongScoreData.goodHits + SongScoreData.perfectHits;

        float percentHits = ((float)totalHitsMade / (float)totalHits) * 100.0f;

        _percentHitText.text = percentHits.ToString("F1") + "%";
        yield return new WaitForSeconds(.3f);
        string rankVal = "F";

        if (percentHits > 40f)
        {
            rankVal = "D";
            if (percentHits > 55f)
            {
                rankVal = "C";
                if (percentHits > 70f)
                {
                    rankVal = "B";
                    if (percentHits > 85)
                    {
                        rankVal = "A";
                        if (percentHits > 95)
                        {
                            rankVal = "S";
                        }
                    }
                }
            }
        }

        _rankText.text = "Rank: " + rankVal;

    }

    IEnumerator ResultDecisionCo(bool isQuitting)
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        yield return new WaitForSeconds(.5f);

        if (isQuitting)
        {
            SceneManager.LoadScene("SongSelect");
        }
        else
        {
            SceneManager.LoadScene("SongStage");
        }
    }
}
