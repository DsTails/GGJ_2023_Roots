using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFade : MonoBehaviour
{
    public static UIFade instance;

    public float fadeSpeed;

    [HideInInspector]
    public bool fading;

    [SerializeField] Image _fadeImage;

    bool _fadingToBlack;
    bool _fadingFromBlack;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fadingToBlack)
        {
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, Mathf.MoveTowards(_fadeImage.color.a, 1f, fadeSpeed * Time.deltaTime));

            if (_fadeImage.color.a == 1f)
            {
                _fadingToBlack = false;
            }
        }

        if (_fadingFromBlack)
        {
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, Mathf.MoveTowards(_fadeImage.color.a, 0f, fadeSpeed * Time.deltaTime));

            if (_fadeImage.color.a == 0f)
            {
                _fadingFromBlack = false;
                fading = false;
                _fadeImage.gameObject.SetActive(false);
            }
        }
    }

    public void FadeToBlack()
    {
        _fadeImage.gameObject.SetActive(true);
        fading = true;
        _fadingToBlack = true;
        _fadingFromBlack = false;
    }

    public void FadeFromBlack()
    {
        _fadingFromBlack = true;
        _fadingToBlack = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_fadeImage.color.a == 1f)
        {
            FadeFromBlack();
        }
    }

}
