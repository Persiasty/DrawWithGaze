using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using IInputHandler = HoloToolkit.Unity.InputModule.IInputHandler;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class DrawWithGaze : MonoBehaviour, IInputHandler
{

    private IInputSource currentInputSource = null;
    private uint currentInputSourceId;
    public float DefaultGazeDistance = 2.0f;
    bool inputDown=false;
    private Plane objPlane;
    [SerializeField] private GameObject _objectToInstantane;
    private Vector3 placementPosition;
    private Transform cameraTransform;

    public void OnInputDown(InputEventData eventData)
    {
        inputDown = true;
        cameraTransform = CameraCache.Main.transform;
        placementPosition = GetPlacementPosition(cameraTransform.position, cameraTransform.forward, DefaultGazeDistance);
        objPlane = new Plane(Camera.main.transform.forward * -1, placementPosition);
    }

    public void OnInputUp(InputEventData eventData)
    {
        inputDown = false;
    }
    private static Vector3 GetPlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
    {
        RaycastHit hitInfo;
        if (SpatialMappingRaycast(headPosition, gazeDirection, out hitInfo))
        {
            return hitInfo.point;
        }
        return GetGazePlacementPosition(headPosition, gazeDirection, defaultGazeDistance);
    }

    private static bool SpatialMappingRaycast(Vector3 origin, Vector3 direction, out RaycastHit spatialMapHit)
    {
        if (SpatialMappingManager.Instance != null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(origin, direction, out hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
            {
                spatialMapHit = hitInfo;
                return true;
            }
        }
        spatialMapHit = new RaycastHit();
        return false;
    }

    /// <summary>
    /// Get placement position either from GazeManager hit or in front of the user as backup
    /// </summary>
    /// <param name="headPosition">Position of the users head</param>
    /// <param name="gazeDirection">Gaze direction of the user</param>
    /// <param name="defaultGazeDistance">Default placement distance in front of the user</param>
    /// <returns>Placement position in front of the user</returns>
    private static Vector3 GetGazePlacementPosition(Vector3 headPosition, Vector3 gazeDirection, float defaultGazeDistance)
    {
        if (GazeManager.Instance.HitObject != null)
        {
            return GazeManager.Instance.HitPosition;
        }
        return headPosition + gazeDirection * defaultGazeDistance;
    }
    // Use this for initialization
    void Start () {
        currentInputSource = GetComponent<IInputSource>();
        
        InputManager.Instance.AddGlobalListener(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (inputDown)
        {
            cameraTransform = CameraCache.Main.transform;
            placementPosition = GetPlacementPosition(cameraTransform.position, cameraTransform.forward, DefaultGazeDistance);
            Ray gazeRay = Camera.main.ScreenPointToRay(placementPosition);
            float rayDistance;
            if (objPlane.Raycast(gazeRay, out rayDistance))
                placementPosition = gazeRay.GetPoint(rayDistance);
            Instantiate(_objectToInstantane, placementPosition, Quaternion.identity);
        }

    }
}
