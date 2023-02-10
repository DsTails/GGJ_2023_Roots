using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    public GameObject outOfBandImage;
    public GameObject audienceLeftImage;

    // Start is called before the first frame update
    void Start()
    {
        if (SongScoreData.lostAudience)
        {
            audienceLeftImage.SetActive(true);
        }
        else
        {
            outOfBandImage.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartSong()
    {
        StartCoroutine(GameOverDecisionCo(false));
    }

    public void QuitToSongSelect()
    {
        StartCoroutine(GameOverDecisionCo(true));
    }

    IEnumerator GameOverDecisionCo(bool isQuitting)
    {
        if (UIFade.instance != null)
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
