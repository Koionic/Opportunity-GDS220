using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    List<string> dialogueStrings = new List<string>();

    [SerializeField]
    DialogueStream groundControlStream, socialMediaStream;

    public static DialogueController instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/SocialMediaDialogue/TestDialogue.txt";

        GrabTextFromFile("/SocialMediaDialogue/TestDialogue", dialogueStrings);

        if (dialogueStrings.Count > 0)
        {
            ChooseRandomText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    ChooseRandomText();
        //}
    }

    void GrabTextFromFile(string dataPath, List<string> dialogueArray)
    {
        string path = Application.dataPath + dataPath + ".txt";

        string[] dialogues = File.ReadAllLines(path);

        if (dialogues.Length > 0)
        {
            for (int i = 0; i < dialogues.Length; i++)
            {
                dialogueArray.Add(dialogues[i]);
            }
        }

    }

    public void QueueGCDialogue(string toBeQueued)
    {
        groundControlStream.AddToQueue(toBeQueued);
    }

    public void QueueSMDialogue(string toBeQueued)
    {
        socialMediaStream.AddToQueue(toBeQueued);
    }

    void ChooseRandomText()
    {
        int randomNum = Random.Range(0, dialogueStrings.Count - 1);

        string dialogueKey = dialogueStrings[randomNum];

        socialMediaStream.AddToQueue(dialogueKey);

        dialogueStrings.Remove(dialogueKey);
    }

    public void ResetStreams()
    {
        dialogueStrings.Clear();

        socialMediaStream.ClearStream();
        groundControlStream.ClearStream();

        GrabTextFromFile("/SocialMediaDialogue/TestDialogue", dialogueStrings);
    }
}
