using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    [SerializeField]
    GameObject sceneManager;

    void Awake()
    {
        if (SceneManager.instance == null)
            Instantiate(sceneManager);
    }

}
