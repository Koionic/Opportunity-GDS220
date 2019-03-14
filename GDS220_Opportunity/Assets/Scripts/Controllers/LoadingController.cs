using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingController : MonoBehaviour
{



    AsyncOperation async;

    float loadingProgress;

    bool loading;
    bool levelHasLoaded;
    bool instantLoad;
    bool levelIsReady;

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
        loading = true;
        levelHasLoaded = false;
        levelIsReady = false;
        instantLoad = instant;
        loadTimer = 0f;
        StartCoroutine(Loading(scene));
    }

    private void FixedUpdate()
    {
        if (levelHasLoaded)
        {
            if (instantLoad)
            {
                levelIsReady = true;
            }
            else
            {
                loadTimer = Time.timeSinceLevelLoad;

                loadingProgress = Mathf.InverseLerp(0, loadTime, loadTimer) * 100;

                if (loadTimer >= loadTime)
                {
                    levelIsReady = true;
                }
            }

            if (levelIsReady)
            {
                if (instantLoad)
                {
                    loading = false;
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    loadTimer = 0f;
                    loading = false;
                }
            }
        }
    }

    IEnumerator Loading(string scene)
    {
        if (instantLoad)
        {
            SceneManager.LoadScene(scene);
        }
        else
        {


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
    }


    public bool IsLoading()
    {
        return loading;
    }

    public bool HasLoaded()
    {
        return levelHasLoaded;
    }

    public bool IsReady()
    {
        return levelIsReady;
    }

    public float LoadProgress()
    {
        return loadingProgress;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        levelHasLoaded = true;
        SceneController.instance.UpdateSceneState();
    }
}