using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HowToPlay : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(LoadMainSceneCo());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadMainSceneCo()
    {
        yield return new WaitForSeconds(2f);
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        yield return new WaitForSeconds(.75f);

        SceneManager.LoadScene("SongStage");
    }
}
