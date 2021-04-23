using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player = null;
    [SerializeField] private Transform _camTransform = null;

    [Header("Variables")]
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] private float _clampXAngle = 80f;
    [SerializeField] private float _zoomMin = 1f;
    [SerializeField] private float _zoomMax = 8f;

    private PlayerControls _playerControls;
    private PlayerControls.CameraRotationActions _cameraRotation;
    
    private float _posOffset = -1f;
    private Vector3 _currentPos;

    private Vector3 _currentRotation;
    private float _pitchRotation = 0.0f;

    private Vector2 _zoomInput;
    private Vector2 _targetZoom;

    private Vector2 _mouseValue;

    //---------------------------------------------------------------------------------------------------------------

    #region Awake / Start / OnEnable / OnDisable

    private void Awake()
    {
        
        
        _playerControls = new PlayerControls();
        _cameraRotation = _playerControls.CameraRotation;

        _targetZoom.y = _camTransform.localPosition.z;

        // Set _mouseValue reference to InputSystem Mouse ActionMap
        _cameraRotation.Mouse.performed += context => _mouseValue = context.ReadValue<Vector2>();
        
        // Set _currentZoom reference to InputSystem Mouse ActionMap
        _cameraRotation.Scroll.performed += context => _zoomInput += context.ReadValue<Vector2>();
    }
    
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerControls.Enable();
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
    }

    #endregion


    private void Update()
    {
        UpdateRotation();
    }

    private void FixedUpdate()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerManager>().gameObject;
        }
        
        Updateposition();
    }

    //---------------------------------------------------------------------------------------------------------------
    #region FollowPlayerPosition & zoom

    private void Updateposition()
    {
        _currentPos = transform.position;

        var targetPos = Vector3.Lerp(_currentPos, GetPlayerPosWithOffset(), .2f);

        transform.position = targetPos;

        CamZoom();
    }

    private void CamZoom()
    {
        var camPos = _camTransform.localPosition;

        _zoomInput = new Vector2(0, _zoomInput.y * -1);
        
        _targetZoom -= _zoomInput.normalized;

        if (_targetZoom.y > -_zoomMin)
        {
            _targetZoom.y = -_zoomMin;
        }
        else if(_targetZoom.y < -_zoomMax)
        {
            _targetZoom.y = -_zoomMax;
        }
        
        var targetPos = Vector3.Lerp(camPos, new Vector3(camPos.x, camPos.y, _targetZoom.y), .08f);

        _camTransform.localPosition = targetPos;
        _zoomInput = Vector2.zero;
    }

    private Vector3 GetPlayerPosWithOffset()
    {
        var playerX = _player.transform.position.x;
        var playerY = _player.transform.position.y;
        var playerZ = _player.transform.position.z;

        var newPos = new Vector3(playerX, playerY + _posOffset, playerZ);

        return newPos;
    }

    #endregion

    #region MouseRotation

    private void UpdateRotation()
    {
        var xRotation = _mouseValue.y * _mouseSensitivity * Time.fixedDeltaTime;
        var yRotation = _mouseValue.x * _mouseSensitivity * Time.fixedDeltaTime;

        _currentRotation = transform.rotation.eulerAngles;

        _pitchRotation -= xRotation;

        _pitchRotation = Mathf.Clamp(_pitchRotation, -1 * _clampXAngle, _clampXAngle);
        
        var targetRotation = new Vector3
            (
            _pitchRotation + xRotation, 
            _currentRotation.y + yRotation, 
            0f
            );
        
        transform.localEulerAngles = targetRotation;

        _mouseValue = Vector2.zero;
    }

    #endregion
}
