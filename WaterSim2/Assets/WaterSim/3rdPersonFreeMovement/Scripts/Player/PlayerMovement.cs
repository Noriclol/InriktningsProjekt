using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    #region Properties

    [Header("References")]
    [SerializeField] private Transform _playerCamTransform = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Material _testShader = null;
    
    private PlayerControls _playerControls;
    private PlayerControls.PlayerMovementActions _playerMovement;
    
    [Header("Settings")]
    [SerializeField] private float _walkSpeed = 2.5f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _acceleration = 0.1f;
    [SerializeField] private float _rotationSpeed = 0.1f;
    [SerializeField] private float _jumpForce = 10f;
    
    // Move
    private Vector2 _moveVector;
    private float _lastSpeed;
    private float _currentSpeed;
    private float _maxSpeed;

    private Vector3 _lastPos;
    private Vector3 _currentPos;
    private Vector3 _targetPos;
    private Quaternion _targetRot;

    private bool _isWalking = true;

    // Jump
    private bool _canJump = true;
    private bool _jumpPressed = false;
    private float _distToGround;
    
    // Rigidbody
    private Rigidbody _rigidbody = null;
    private float _rbMass;

    #endregion
    
    //---------------------------------------------------------------------------------------------------------------
    #region Awake / Start / OnEnable / OnDisable

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerMovement = _playerControls.PlayerMovement;

        _rigidbody = GetComponent<Rigidbody>();
        _rbMass = _rigidbody.mass;

        _maxSpeed = _walkSpeed;
        
        // Set _moveVector reference to InputSystem Move ActionMap
        _playerMovement.Move.performed += context => _moveVector = context.ReadValue<Vector2>();
        _playerMovement.Move.canceled += context => _moveVector = Vector2.zero;
        
        // Set Sprint reference to InputSystem Move ActionMap
        _playerMovement.Sprint.performed += context => PlayerSprint();
        _playerMovement.Sprint.canceled += context => _isWalking = true;
        
        // Jump
        _playerMovement.Jump.performed += context => _jumpPressed = true;
        _playerMovement.Jump.canceled += context => CanJump();
        
        _distToGround = GetComponent<Collider>().bounds.extents.y;
    }
    
    private void OnEnable()
    {
        _playerControls.Enable();
        
        _currentPos = transform.position;
    }
    
    private void OnDisable()
    {
        _playerControls.Disable();
    }

    #endregion
    
    //---------------------------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        PlayerWalk();
        PlayerMove();
        PlayerRotate();
        
        Jump();
    }

    private void Update()
    {
        _currentSpeed = _rigidbody.velocity.magnitude;

        //TestModifyShader();
        GetTargetPosition();
        
        _animator.SetFloat("Speed", _currentSpeed);
    }

    private void TestModifyShader()
    {
        _testShader.SetFloat("Vector1_abb1e7bfdc5b42628eae3c817c6a2a64", Math.Abs(_currentSpeed * 2));
    }

    #region Movement / Jump
    
    //---------------------------------------------------------------------------------------------------------------
    private void GetTargetPosition()
    {
        _currentPos = transform.position;

        float posX = _moveVector.x * _acceleration;
        float posZ = _moveVector.y * _acceleration;
        
        Vector3 forward = _playerCamTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 moveDirection = (posX * right + posZ * forward);

        _targetPos = moveDirection * _rbMass;
    }
    
    //Move
    private void PlayerMove()
    {
        float tempY = _rigidbody.velocity.y;

        _rigidbody.AddForce(new Vector3(_targetPos.x, 0f, _targetPos.z));
        
        //clamp speed
        if(_currentSpeed > _maxSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
        }

        //restore y speed (unclamp)
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, tempY, _rigidbody.velocity.z);
    }
    
    // Run -> Walk deceleration
    private void PlayerSprint()
    {
        _maxSpeed = _runSpeed;
        _isWalking = false;
    }

    private void PlayerWalk()
    {
        if (_isWalking)
        {
            var speed = Mathf.Lerp(_currentSpeed, _walkSpeed, .08f);

            _maxSpeed = speed;
        }
    }

    // Look at move direction
    private void PlayerRotate()
    {
        var rotation = transform.rotation;
        var rotYSubtract = new Vector3(0, _rigidbody.velocity.y, 0);
        var rotTarget = _rigidbody.velocity - rotYSubtract;
        
        float absX = Mathf.Abs(_targetPos.x);
        float absZ = Mathf.Abs(_targetPos.z);
        
        if (absX > 0.1f || absZ > 0.1f)
        {
            _targetRot = Quaternion.LookRotation(new Vector3(rotTarget.x * 5, rotTarget.y, rotTarget.z * 5).normalized, Vector3.up);
        }
        
        rotation = Quaternion.Lerp(rotation, _targetRot, _rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.rotation = rotation;
    }
    
    //---------------------------------------------------------------------------------------------------------------
    //Jump
    private void Jump()
    {
        Vector3 jf = new Vector3(0f, _jumpForce, 0f);
        
        if (_jumpPressed && _canJump && IsGrounded())
        {
            _canJump = false;
            _rigidbody.AddForce(jf, ForceMode.Impulse);
            _animator.SetBool("IsGrounded", false);
        }
    }
    
    // isGrounded Raycast
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, - Vector3.up, _distToGround + 0.1f);
    }
    
    // _canJump = true if Spacebar released
    private void CanJump()
    {
        _canJump = true;
        _jumpPressed = false;
    }
    
    #endregion
    
    //---------------------------------------------------------------------------------------------------------------

    #region Collision

    private void OnCollisionEnter(Collision other)
    {
        // Stop character from rotating on collisions
        _rigidbody.angularVelocity = Vector3.zero;
        
        // Set animator isGrounded
        if (IsGrounded())
        {
            _animator.SetBool("IsGrounded", true);
        }
    }

    #endregion
    
    //---------------------------------------------------------------------------------------------------------------
    #region InputSystem callback

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }
        
    // public bool OnJump(InputAction.CallbackContext context)
    // {
    //     {
    //         throw new NotImplementedException();
    //     }
    // }

    #endregion
    
}
