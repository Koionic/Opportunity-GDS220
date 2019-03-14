using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private void Start()
    {
        SceneController.instance.MainMenu();
        Destroy(this);
    }
}
