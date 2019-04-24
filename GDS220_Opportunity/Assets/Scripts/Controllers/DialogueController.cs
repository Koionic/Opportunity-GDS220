using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    List<string> dialogueStrings = new List<string>();

    [SerializeField]
    DialogueStream socialMediaStream;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/SocialMediaDialogue/TestDialogue.txt";

        string[] dialogues = File.ReadAllLines(path);

        for (int i = 0; i < dialogues.Length; i++)
        {
            dialogueStrings.Add(dialogues[i]);
        }

        if (dialogueStrings.Count > 0)
        {
            ChooseRandomText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChooseRandomText();
        }
    }

    void ChooseRandomText()
    {
        int randomNum = Random.Range(0, dialogueStrings.Count - 1);

        string dialogueKey = dialogueStrings[randomNum];

        socialMediaStream.AddToQueue(dialogueKey);

        dialogueStrings.Remove(dialogueKey);
    }
}
