using UnityEngine;

public class ProceduralMapCamera : MonoBehaviour
{
    [SerializeField] private float _panSpeed = 3;
    [SerializeField] private float _zoomSpeed = 3;
    [SerializeField] private float _minCameraSize = 5;

    private Camera _cam;
    private ProceduralMapGenerator _generator;

    private Vector3 _targetVector;
    private int _targetCameraSize;

    private bool _cameraMove;
    private bool _cameraZoom;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _generator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<ProceduralMapGenerator>();
    }

    private void FixedUpdate()
    {
        if (_cameraMove)
        {
            CameraMove();
        }

        if (_cameraZoom)
        {
            CameraZoom();
        }
    }

    private void CameraMove()
    { 
        if (transform.position == _targetVector)
        {
            _cameraMove = false;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _targetVector, _panSpeed * Time.deltaTime);
    }

    private void CameraZoom()
    {
        //Change to < 0-1 or > 0-1
        if (_cam.orthographicSize == _targetCameraSize)
        {
            _cameraZoom = false;
            return;
        }

        if (_cam.orthographicSize < _minCameraSize)
        {
            _cam.orthographicSize = _minCameraSize;
        }

        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetCameraSize, _zoomSpeed * Time.deltaTime);
    }

    // Called by UI Button
    public void StartCameraMovement()
    {
        if (_generator.mapLength <= 0)
        {
            return;
        }

        Vector3 middleOfMap = _generator.GetTileMapAverageMidPoint();
        _targetVector = new Vector3(middleOfMap.x, middleOfMap.y, -10);
        _targetCameraSize = Mathf.RoundToInt(_generator.mapLength / 2.5f);
        _cameraZoom = true;
        _cameraMove = true;
    }
}
