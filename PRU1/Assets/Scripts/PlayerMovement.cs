using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    private bool _isFacingRight = true;
    [Space]
    [Header("Dash")]
    [SerializeField] private bool _canDash;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;


    [Space]
    [Header("Misc")]
    [SerializeField] private float _speed = 15f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private TrailRenderer tr;

    [Space]
    [Header("Jump")]
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferTimeCounter;
    private float _coyoteTimeCounter;
    private float _fallMultiplier = 7f;
    private float _jumpVelocityFallOff = 8f;
    
    [SerializeField] private int extraJump = 1;
    [SerializeField] private int availableJump;

    [Space]
    [Header("Wall Tech")]
    [SerializeField] private bool _isWallSliding;
    [SerializeField] private bool _isWallJumping;
    [SerializeField] private float _wallSlidingSpeed;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDirection;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    private UnityEngine.Vector2 _wallJumpingPower = new UnityEngine.Vector2(6f, 15f);

    private float xRaw, yRaw;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isDashing)
        {
            return;
        }
        
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");

        BasicMovement();

        Jump();

        Dash();

        if (!_isWallJumping)
        {
            Flip();
        }

        WallSlide();

        WallJump();
    }
    private void FixedUpdate()
    {
        if (_isDashing)
        {
            return;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private bool IsWalled()
    {
        
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    private void BasicMovement()
    {
        if (!_isWallJumping)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(Input.GetAxis("Horizontal") * _speed, _rb.linearVelocityY);
        }
        else
        {
            _rb.linearVelocity = UnityEngine.Vector2.Lerp(_rb.linearVelocity, new UnityEngine.Vector2(_wallJumpingDirection * _speed, _rb.linearVelocityY), 0.5f * Time.deltaTime);
        }
    }
    private void Jump()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
            availableJump = extraJump;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") || (Input.GetButtonDown("Jump") && _isDashing))
        {
            _jumpBufferTimeCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimeCounter -= Time.deltaTime;
        }
        if (_jumpBufferTimeCounter > 0f && _coyoteTimeCounter > 0f)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            availableJump--;
        }
        else if (Input.GetButtonDown("Jump") && _coyoteTimeCounter <= 0f && availableJump > 0 && canDoubleJump)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            availableJump--;
        }

        if (Input.GetButtonUp("Jump") && _rb.linearVelocityY > 0f || _rb.linearVelocityY < _jumpVelocityFallOff)
        {
            _rb.linearVelocity += _fallMultiplier * Physics2D.gravity.y * UnityEngine.Vector2.up * Time.deltaTime;
            _coyoteTimeCounter = 0f;
        }
    }
    private void Flip()
    {
        if (_isFacingRight && xRaw < 0f || !_isFacingRight && xRaw > 0f)
        {
            UnityEngine.Vector3 localScale = transform.localScale;
            _isFacingRight = !_isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Dash()
    {
        if (IsGrounded())
        {
            _canDash = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(ExecuteDash());
        }
    }
    private IEnumerator ExecuteDash()
    {
        _canDash = false;
        _isDashing = true;
        float dashNormalizer = 0.707f;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        // rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        if (yRaw != 0 && xRaw != 0)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(transform.localScale.x * _dashingPower * dashNormalizer, yRaw * _dashingPower * dashNormalizer);
        }
        else if (yRaw != 0 && xRaw == 0)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(0f, yRaw * _dashingPower);
        }
        else
        {
            _rb.linearVelocity = new UnityEngine.Vector2(transform.localScale.x * _dashingPower, yRaw * _dashingPower);
        }
        float yRec = yRaw;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime * dashNormalizer);
        tr.emitting = false;
        if (yRec > 0)
        {
            _rb.linearVelocity = new UnityEngine.Vector2(_rb.linearVelocityX, 0f);
        }
        _rb.gravityScale = originalGravity;

        //rb.linearVelocity = new Vector2(0f, 0f);
        _isDashing = false;
    }
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && xRaw != 0 && _rb.linearVelocityY <= 0)
        {
            _rb.linearVelocityY *= _wallSlidingSpeed;
            _isWallSliding = true;
        }
        else
        {
            _isWallSliding = false;
        }
    }
    private void WallJump()
    {
        
        if (IsWalled() && !IsGrounded())
        {
            _isWallJumping = false;
            _wallJumpingDirection = -transform.localScale.x;
            _wallJumpingCounter = _wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            _wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            _rb.linearVelocity = new UnityEngine.Vector2(_wallJumpingDirection * _speed, _jumpSpeed);

            _wallJumpingCounter = 0f;

            if (_wallJumpingDirection != transform.localScale.x)
            {
                UnityEngine.Vector3 localScale = transform.localScale;
                _isFacingRight = !_isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), _wallJumpingDuration);
        }
    }
    private void StopWallJumping()
    {
        _isWallJumping = false;
    }
    
}
