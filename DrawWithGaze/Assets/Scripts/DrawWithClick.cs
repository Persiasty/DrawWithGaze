using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class DrawWithClick : MonoBehaviour, IInputHandler {

    [SerializeField] private Transform _cameraPivotTransform;

    [SerializeField] private Material _lineMaterial;
    private bool _isDrawing;
    [SerializeField] private float _spawnDistance=4;

    private LineRenderer _lineRenderer;
    private List<Vector3> _linePoints;
    private int _linePointsCount = 0;
    

    public void OnInputDown(InputEventData eventData)
    {
        _isDrawing = true;
        PrepareForNewLine();
    }

    private void PrepareForNewLine()
    {
        GameObject _objectWithRenderer = Instantiate(new GameObject());
        _lineRenderer = _objectWithRenderer.AddComponent<LineRenderer>();
        _lineRenderer.material = _lineMaterial;
        SetupLine(_lineRenderer);

        _linePoints = new List<Vector3>();
    }

    void SetupLine(LineRenderer line)
    {
        line.sortingLayerName = "OnTop";
        line.sortingOrder = 5;
        line.SetWidth(0.1f, 0.1f);
        line.numCapVertices=1;
        line.numCornerVertices = 1;
        line.widthMultiplier = 0.2f;
        line.useWorldSpace = true;
        line.material = _lineMaterial;
        _linePointsCount = 0;
    }

    public void OnInputUp(InputEventData eventData)
    {
        _isDrawing = false;
    }

    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    private void FixedUpdate()
    {
        if(_isDrawing)
        {
            Vector3 pointPosition = _cameraPivotTransform.position + _cameraPivotTransform.forward*_spawnDistance;
            Debug.Log("Adding new point at: " + pointPosition);
            _linePoints.Add(pointPosition);
            _linePointsCount++;
            _lineRenderer.SetVertexCount(_linePointsCount);
            _lineRenderer.SetPositions(_linePoints.ToArray());
        }
    }

}
