using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CameraQuest : Quest
{
    bool potentialPhoto, potentialPhotoCorrect;

    public override void StartQuest()
    {
        base.StartQuest();
    }

    // Update is called once per frame
    public override void QuestUpdate()
    {
        base.QuestUpdate();

    }

    public override void CheckPhoto(Texture2D newPhoto, bool correct)
    {
        if (tutorialQuest)
        {
            UIController.instance.HideTutorialText();
        }
        potentialPhoto = true;
        potentialPhotoCorrect = correct;
        //UIController.instance.ShowNewPhoto(newPhoto);
    }
}
