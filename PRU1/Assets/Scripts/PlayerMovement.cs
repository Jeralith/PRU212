using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
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

    [SerializeField] private int _extraJump = 1;
    [SerializeField] private int _availableJump;

    [Space]
    [Header("Wall Tech")]
    //[SerializeField] private bool _isWallSliding;
    [SerializeField] private bool _isWallJumping;
    [SerializeField] private float _wallSlidingSpeed;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDirection;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    [SerializeField] private float _wallJumpingLerp = 10f;
    private Vector2 _wallJumpingPower = new Vector2(6f, 15f);

    private float xRaw, yRaw;

    #endregion
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
        //if player is walljumping, use a slightly different movement mechanic
        if (!_isWallJumping)
        {
            _rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * _speed, _rb.linearVelocityY);
        }
        else
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, new Vector2(_wallJumpingDirection * _speed, _rb.linearVelocityY), _wallJumpingLerp * Time.deltaTime);
        }
    }
    private void Jump()
    {
        //initialize jump buffering and coyote time
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
            //reset double jump when player is grounded
            _availableJump = _extraJump;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        //also activate buffer is player is dashing
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
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            //if player touches wall, use wall jump instead 
            if (!IsWalled())
            {
                _availableJump--;
            }
        }
        //double jump condition
        else if (Input.GetButtonDown("Jump") && _coyoteTimeCounter <= 0f && _availableJump > 0 && canDoubleJump)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            if (!IsWalled())
            {
                _availableJump--;
            }
        }
        //apply downward force for snappier jump feeling
        if (Input.GetButtonUp("Jump") && _rb.linearVelocityY > 0f || _rb.linearVelocityY < _jumpVelocityFallOff)
        {
            _rb.linearVelocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            _coyoteTimeCounter = 0f;
        }
    }
    //flip player's entire model horizontally when moving opposite direction
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
        //dash is refilled when player touches ground
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
        //math time: player dashing 1 direction => distance is 1 unit. player dashing 2 dirs (diagonal), distance is sqrt(2). this shit normalize diagonal dashes back to 1.
        float dashNormalizer = 0.707f;
        float originalGravity = _rb.gravityScale;
        //disable gravity for total straight dash movement
        _rb.gravityScale = 0f;
        //diagonal cases
        if (yRaw != 0 && xRaw != 0)
        {
            _rb.linearVelocity = new Vector2(transform.localScale.x * _dashingPower * dashNormalizer, yRaw * _dashingPower * dashNormalizer);
        }
        //up/down cases
        else if (yRaw != 0 && xRaw == 0)
        {
            _rb.linearVelocity = new Vector2(0f, yRaw * _dashingPower);
        }
        else
        //everything else
        {
            _rb.linearVelocity = new Vector2(transform.localScale.x * _dashingPower, yRaw * _dashingPower);
        }
        float yRec = yRaw;
        //draw trial
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime * dashNormalizer);
        tr.emitting = false;
        if (yRec > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0f);
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
            //_isWallSliding = true;
        }
        else
        {
            //_isWallSliding = false;
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
            _rb.linearVelocity = new Vector2(_wallJumpingDirection * _speed, _jumpSpeed);

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
