using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandMember : MonoBehaviour, IObserver
{
    // Start is called before the first frame update

    public Subject trackManagerSubject;

    Animator _animator;

    public AnimationClip[] playStates;

    void Start()
    {
        trackManagerSubject.AddObserver(this);
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNotify(NoteEnum noteData)
    {
        if(noteData == NoteEnum.songStart)
        {
            if (_animator.HasState(0, Animator.StringToHash("Playing"))){
                _animator.Play("Playing");
            }
        } else if(noteData == NoteEnum.songEnd)
        {
            _animator.Play("Idle");
        }
        else if(noteData.ToString().Contains("Hit"))
        {
            if(playStates.Length != 0)
            {
                int randomState = Random.Range(0, playStates.Length);

                _animator.Play(playStates[randomState].name);
            }
        }
    }
}
