using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SongMenu : MonoBehaviour
{
    SongData _selectedSong;


    public SongData[] songDatas;

    List<SongButton> _songButtons = new List<SongButton>();

    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] SongButton _songListButtons;
    [SerializeField] GameObject _buttonParent;


    // Start is called before the first frame update
    void Start()
    {
        //Decide whether or not the list is populated dynamically
        for(int i = 0; i < songDatas.Length; i++)
        {
            SongButton listButton = Instantiate(_songListButtons, _buttonParent.transform);
            listButton.buttonIndex = i;
            listButton.songNameText.text = songDatas[i].songName;
            listButton.parentMenu = this;
            _songButtons.Add(listButton);
        }

        _songButtons[0].Press();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectSong(int index)
    {
        _selectedSong = songDatas[index];
        _nameText.text = _selectedSong.songName;
        _descriptionText.text = _selectedSong.songDescription;
    }

    public void PlaySong()
    {
        SongSelectData.loadedSong = _selectedSong.songClip;
        SongSelectData.tempoMapLocation = _selectedSong.tempoMapLocation;
        SongSelectData.loadedNormalBass = _selectedSong.bassClipNormal;
        SongSelectData.loadedSoloBass = _selectedSong.bassClipSolo;
        SceneManager.LoadScene("HowToPlay");
    }
}
