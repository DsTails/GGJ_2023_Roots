using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour
{
    List<IObserver> _observers = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }
    //
    protected void NotifyObservers(NoteEnum noteData = NoteEnum.noteMiss)
    {
        if (_observers.Count != 0)
        {
            _observers.ForEach((observer) =>
            {
                observer.OnNotify(noteData);
            });
        }
    }

    public int GetObserverNum()
    {
        return _observers.Count;
    }

    public void GetObserverNames()
    {
        _observers.ForEach((observer) =>
        {
            Debug.Log(observer);
        });
    }

}
