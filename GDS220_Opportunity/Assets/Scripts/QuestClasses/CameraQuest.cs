using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CameraQuest : Quest
{
    public Transform photoTarget;

    public override void StartQuest()
    {
        base.StartQuest();
        roverController.photoTaken.AddListener(CheckPhoto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckPhoto()
    {
        
    }
}
