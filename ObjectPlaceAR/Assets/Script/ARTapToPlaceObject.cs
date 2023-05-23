using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
//using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    public GameObject info;
    private Vector2 startPos;
    private Vector2 direction;
    private bool placePoseIsValid = false;
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placePoseIsValid && placementIndicator.activeSelf && Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                //When a touch has first been detected, change the message and record the starting position
                case TouchPhase.Began:
                    // Record initial touch position.
                    startPos = touch.position;
                    break;

                //Determine if the touch is a moving touch
                case TouchPhase.Moved:
                    // Determine direction by comparing the current touch position with the initial one
                    direction = touch.position - startPos;
                    placementIndicator.transform.Rotate(0, direction.x / 5, 0, Space.Self);
                    break;

                case TouchPhase.Ended:
                    // Report that the touch has ended when it ends
                    direction = touch.position - startPos;
                    if (direction == Vector2.zero)
                    {
                        PlaceObject();
                    }
                    break;
            }
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementIndicator.transform.position, placementIndicator.transform.rotation);
    }
    private void UpdatePlacementIndicator()
    {
        if (placePoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementIndicator.transform.rotation);
        }
        else
        {
            info.GetComponent<UnityEngine.UI.Text>().text = "No hit";
            placementIndicator.SetActive(false);
        }
    }
    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hitsRaycastManager = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hitsRaycastManager, TrackableType.All);
        placePoseIsValid = hitsRaycastManager.Count > 0;

        if (placePoseIsValid)
        {
            info.GetComponent<UnityEngine.UI.Text>().text = System.Enum.GetName(typeof(TrackableType), hitsRaycastManager[0].hitType);
            placementPose = hitsRaycastManager[0].pose;
        }
    }
}
