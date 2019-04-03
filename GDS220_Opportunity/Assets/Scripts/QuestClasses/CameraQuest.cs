using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CameraQuest : Quest
{
    public Transform photoTarget;

    bool potentialPhoto, potentialPhotoCorrect;

    public override void StartQuest()
    {
        base.StartQuest();
    }

    // Update is called once per frame
    public override void QuestUpdate()
    {
        Debug.Log("updating");
        if (potentialPhoto)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                 if (potentialPhotoCorrect)
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
        potentialPhoto = true;
        potentialPhotoCorrect = correct;
        UIController.instance.ShowNewPhoto(newPhoto);
    }

    public override Transform GetPhotoTarget()
    {
        return photoTarget;
    }
}
