using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        
        else if (instance != this)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
