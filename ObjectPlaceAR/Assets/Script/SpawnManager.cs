using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class SpawnManager : MonoBehaviour
{
    [Header("ItemData")]
    [SerializeField] Image imgItem;
    [SerializeField] GameObject buttonRemove;
    [SerializeField] Item[] items;

    [Header("Refrences")]
    [SerializeField] Camera cam;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARRaycastManager raycastManager;

    public static int currItem;
    private GameObject spwanedObj;
    private List<ARRaycastHit> hits;

    private Vector2 fTouch;
    private Vector2 sTouch;
    private float currDist;
    private float preDist;
    private bool ftClick = true;

    private void Start()
    {
        Init();
        ButtonRemove();
    }

    private void Update()
    {
        if (!spwanedObj) Spwan();
        else Zoom();
    }


    private void Init()
    {
        UpdateUI();
        hits = new List<ARRaycastHit>();
    }

    private void UpdateUI()
    {
        imgItem.sprite = items[currItem].sprite;
    }

    private void Spwan()
    {
        if (Input.touchCount == 0) return;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);

        if (raycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            if ((Input.GetTouch(0).phase == TouchPhase.Began) && Physics.Raycast(ray, out hit))
            {
                spwanedObj = Instantiate(items[currItem].prefab, hits[0].pose.position, Quaternion.identity);
                ActiveObjects(false);
            }
        }
    }

    private void Zoom()
    {
        if (Input.touchCount > 1 && spwanedObj)
        {
            fTouch = Input.GetTouch(0).position;
            sTouch = Input.GetTouch(1).position;
            currDist = sTouch.magnitude - fTouch.magnitude;
            if (ftClick)
            {
                preDist = currDist;
                ftClick = false;
            }
            if (currDist != preDist)
            {
                Vector3 scale_value = spwanedObj.transform.localScale * (currDist / preDist);
                spwanedObj.transform.localScale = scale_value;
                preDist = currDist;

            }
        }
        else
        {
            ftClick = true;
        }
    }

    private void ActiveObjects(bool isActive)
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(isActive);
        }

        raycastManager.enabled = isActive;
        planeManager.enabled = isActive;

        buttonRemove.SetActive(!isActive);
    }

    public void ButtonRemove()
    {
        if (spwanedObj) Destroy(spwanedObj);
        spwanedObj = null;
        ActiveObjects(true);
    }

    public void ButtonChange()
    {
        ButtonRemove();
        currItem = (currItem + 1) % items.Length;
        UpdateUI();
    }
}