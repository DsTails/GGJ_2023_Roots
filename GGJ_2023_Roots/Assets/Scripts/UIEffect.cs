using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffect : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyObject), .75f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
