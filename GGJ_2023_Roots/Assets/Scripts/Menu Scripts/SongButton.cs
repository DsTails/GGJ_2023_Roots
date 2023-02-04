using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongButton : MonoBehaviour
{
    [HideInInspector]
    public SongMenu parentMenu;
    public int buttonIndex;

    public TextMeshProUGUI songNameText; 

    public void Press()
    {
        parentMenu.SelectSong(buttonIndex);
    }
}
