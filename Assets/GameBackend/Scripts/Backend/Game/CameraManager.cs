using System.Collections;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Camera mainCamera;

    public RTS_Camera mapSpectateComponent;

    public Vector3 mapViewPosition;
    public Vector3 mapViewRotation;

    public Vector3 spectateOffset;
    public Vector3 spectateRotation;

    void Awake()
    {
        mapViewPosition = transform.position;
        mapViewRotation = transform.localEulerAngles;
    }

    public int index = -1;

    void Update()
    {
        SetPosition();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mapSpectateComponent.enabled)
            {
                mapSpectateComponent.enabled = false;
                mapViewPosition = mainCamera.transform.position;
                mapViewRotation = mainCamera.transform.localEulerAngles;
            }
            else
            {
                Rotate();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!mapSpectateComponent.enabled)
            {
                mainCamera.transform.position = mapViewPosition;
                mainCamera.transform.localEulerAngles = mapViewRotation;
                mapSpectateComponent.enabled = true;
            }
        }
    }

    void SetPosition()
    {
        if (mapSpectateComponent.enabled)
        {
            return;
        }

        //Get an agent
        Transform currentTransform = (index <= TeamManager.Instance.GetRedTeam().agents.Count - 1) ? 
            TeamManager.Instance.GetRedTeam().agents[index].transform : TeamManager.Instance.GetBlueTeam().
            agents[index - TeamManager.Instance.GetRedTeam().agents.Count].transform;
        currentTransform.GetComponent<Planner>().MakeSelectedAgent();

        mainCamera.transform.position = currentTransform.transform.position - (currentTransform.forward * spectateOffset.z) + (Vector3.up * spectateOffset.y);
        mainCamera.transform.localEulerAngles = currentTransform.localEulerAngles + spectateRotation;
    }

    void Rotate()
    {
        index++;

        if (index >= TeamManager.Instance.GetRedTeam().agents.Count + TeamManager.Instance.GetBlueTeam().agents.Count)
        {
            index = 0;
        }
    }
}
