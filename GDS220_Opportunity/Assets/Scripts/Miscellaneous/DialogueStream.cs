using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueStream : MonoBehaviour
{
    List<string> queue = new List<string>();

    List<TextMeshProUGUI> stream = new List<TextMeshProUGUI>();

    [SerializeField]
    TextMeshProUGUI entryPrefab;

    [SerializeField]
    float minimumRefreshTime, maximumRefreshTime;

    float refreshTimer, timeToRefresh;

    [SerializeField]
    int maxLines;

    [SerializeField]
    ScrollRect scrollRect;

    bool newContent;

    public void AddToQueue(string newString)
    {
        queue.Add(newString);
    }

    void Update()
    {
        refreshTimer += Time.deltaTime;

        if (refreshTimer >= timeToRefresh)
        {
            UpdateStream();

            timeToRefresh = Random.Range(minimumRefreshTime, maximumRefreshTime);
            refreshTimer = 0f;
        }
    }

    void UpdateStream()
    {
        if (queue.Count > 0)
        {
            if (queue[0] != null)
            {
                TextMeshProUGUI newEntry = Instantiate(entryPrefab, transform);
                newEntry.text = queue[0];

                stream.Add(newEntry);
                queue.RemoveAt(0);

                if (stream.Count > maxLines)
                {
                    Destroy(stream[0]);
                    stream.RemoveAt(0);
                }

                StartCoroutine(ResetToBottom());
            }
            else
            {
                stream.RemoveAt(0);
            }
        }
    }

    IEnumerator ResetToBottom()
    {
        yield return null;
        scrollRect.normalizedPosition = Vector2.zero;
    }
}
