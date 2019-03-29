using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCamera : MonoBehaviour
{
    [SerializeField]
    float minZoom = 40f, maxZoom = 90f, zoomSpeed = 3f;

    public float zoomPercent;

    [SerializeField]
    float viewportMargin;

    [SerializeField]
    float distanceMargin;

    Camera roverCamera;

    Camera photoCamera;

    [SerializeField]
    Transform cameraTarget;

    bool targetTooFar;
    bool targetInView;
    bool targetObscured;

    bool takePhotoNextFrame;

    RoverController roverController;

    // Start is called before the first frame update
    void Start()
    {
        roverController = FindObjectOfType<RoverController>();
        roverCamera = roverController.fpsCamera;
        photoCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roverController.cameraMode)
        {
            ZoomCamera(Input.GetAxis("CameraHorizontal"));

            if (QuestController.instance != null)
            {
                if (QuestController.instance.currentQuestType == QuestType.Photo)
                {

                }
            }
            else
            {
                CheckAngleOfTarget(cameraTarget);
                CheckLineOfSight(cameraTarget);
            }

            if (Input.GetMouseButtonDown(0))
            {
                TriggerPhoto(Screen.width, Screen.height);
            }
        }
    }

    void ZoomCamera (float zoomAmount)
    {
        if (zoomAmount != 0f)
        {
            float deltaZoom = Mathf.Clamp(photoCamera.fieldOfView + (zoomAmount * Time.deltaTime * zoomSpeed), minZoom, maxZoom);
            photoCamera.fieldOfView = deltaZoom;
            roverController.fpsCamera.fieldOfView = photoCamera.fieldOfView;

            zoomPercent = 1f - Mathf.InverseLerp(minZoom, maxZoom, photoCamera.fieldOfView);
        }
    }

    void CheckAngleOfTarget(Transform target)
    {
        //creates a vector to compare against the vector between the player and the target
        Vector3 normalisedHeading = transform.forward;
        Vector3 normalisedCameraVector = (target.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(normalisedHeading, normalisedCameraVector);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        float searchAngle = photoCamera.fieldOfView - viewportMargin;

        if (angle < searchAngle)
        {
            targetInView = true;
        }
        targetInView = false;
    }

    //function that checks if the player is in the enemies physical line of sight
    void CheckLineOfSight(Transform target)
    {
        //creates a ray facing the player
        Ray ray = new Ray(transform.position, (target.position - transform.position));
        RaycastHit raycast;

        //checks if anything is in the players direction
        if (Physics.Raycast(ray, out raycast))
        {
            //sets the bool to true if the player is hit by the ray
            if (raycast.collider.CompareTag("CameraTarget"))
            {
                targetObscured = false;

                if ((target.position - transform.position).magnitude > distanceMargin)
                {
                    targetTooFar = true;
                }
                else
                {
                    targetTooFar = false;
                }
            }
            else if (targetInView)
            {
                targetObscured = true;
            }
        }
    }

    private void OnPostRender()
    {
        if (takePhotoNextFrame)
        {
            takePhotoNextFrame = false;
            TakePhoto();
        }
    }

    private void TriggerPhoto(int width, int height)
    {
        photoCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takePhotoNextFrame = true;
    }

    void TakePhoto()
    {
        RenderTexture renderTexture = photoCamera.targetTexture;

        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        renderResult.ReadPixels(rect, 0, 0);

        byte[] byteArray = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshot.png", byteArray);

        RenderTexture.ReleaseTemporary(renderTexture);
        photoCamera.targetTexture = null;
    }

}
