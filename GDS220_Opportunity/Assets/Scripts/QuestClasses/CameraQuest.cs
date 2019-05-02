﻿using System.Collections;
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

        if (potentialPhoto)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (potentialPhotoCorrect)
                {
                    questData.success = true;
                }

                UIController.instance.DeleteNewPhoto();

                if (tutorialQuest)
                {
                    if (potentialPhotoCorrect)
                    {
                        EndQuest();
                    }
                    else
                    {
                        UIController.instance.ShowTutorialText("Space Station Not In View, Try Again");
                    }
                }
                else
                {
                    EndQuest(); 
                }

            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                UIController.instance.DeleteNewPhoto();
                potentialPhotoCorrect = false;
                potentialPhoto = false;
            }
        }
    }

    public override void CheckPhoto(Texture2D newPhoto, bool correct)
    {
        if (tutorialQuest)
        {
            UIController.instance.HideTutorialText();
        }
        potentialPhoto = true;
        potentialPhotoCorrect = correct;
        UIController.instance.ShowNewPhoto(newPhoto);
    }
}
