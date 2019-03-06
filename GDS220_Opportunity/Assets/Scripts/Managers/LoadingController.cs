using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingController : MonoBehaviour
{

    [SerializeField] GameObject loadingScreen;
    [SerializeField] TextMeshProUGUI textMesh;

    AsyncOperation async;

    bool levelHasLoaded;
    bool instantLoad;

    public static LoadingController instance = null;

    [SerializeField] float loadTime = 5f;
    float loadTimer;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(this);
    }

    public void StartLoadingScreen(string scene, bool instant)
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        levelHasLoaded = false;
        instantLoad = instant;
        loadTimer = 0f;
        StartCoroutine(Loading(scene));
    }

    private void Update()
    {
        if (levelHasLoaded)
        {
            if (instantLoad)
            {
                loadingScreen.SetActive(false);
            }
            else
            {
                loadTimer += Time.deltaTime;

                float loadingProgress = Mathf.InverseLerp(0, loadTime, loadTimer) * 100;
                textMesh.text = loadingProgress.ToString("F") + "%";

                if (loadTimer >= loadTime)
                {
                    textMesh.color = Color.green;

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        loadTimer = 0f;
                        loadingScreen.SetActive(false);
                    }
                }
            }
        }
    }

    IEnumerator Loading(string scene)
    {
        //enables the black screen and other ui
        loadingScreen.SetActive(true);
        //starts loading the next scene
        async = SceneManager.LoadSceneAsync(scene);
        //prevents the scene from being activated before it's fully loaded
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
           
            //async describes 0.9 as fully loaded
            if (async.progress == 0.9f)
            {
                async.allowSceneActivation = true;

                yield break;

            }
            yield return null;
        }
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        levelHasLoaded = true;
    }
}
