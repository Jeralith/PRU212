using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Fundementals")]
    [SerializeField] private Rigidbody2D _rb;
    private bool _isFacingRight = true;
    [SerializeField] private float _speed = 15f;

    [Space]
    [Header("Dash")]
    [SerializeField] private bool _canDash;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    private float _dashNormalizer = 0.707f;

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
    [SerializeField] private bool _isWallSliding;
    [SerializeField] private bool _isWallJumping;
    [SerializeField] private float _wallSlidingSpeed;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDirection;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    [SerializeField] private float _wallJumpingLerp = 10f;
    private Vector2 _wallJumpingPower = new Vector2(6f, 15f);

    [Space]
    [Header("Collisions")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [Space]
    [Header("Misc")]
    [SerializeField] private float _freezeDuration = 1f;
    [SerializeField] private bool _isFrozen;
    [SerializeField] private float _pendingFreezeDuration = 0f;
    [SerializeField] private TrailRenderer tr;
    private bool _wasGrounded;
    [SerializeField] private ParticleSystem slideParticle;
    [SerializeField] private ParticleSystem groundParticle;
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private float _dashDustSpeed = 10f;
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
        if (_pendingFreezeDuration > 0 && !_isFrozen)
        {
            StartCoroutine(ExecuteFreeze());
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
        WallParticle();
        _wasGrounded = IsGrounded();
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
        if (!_wasGrounded && IsGrounded())
        {
            MakeDust();
        }
    }
    private void Jump()
    {
        var main = groundParticle.main;
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
        //starts ground jump
        if (_jumpBufferTimeCounter > 0f && _coyoteTimeCounter > 0f)
        {
            MakeDust();
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
            
            MakeDust();
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            if (!IsWalled())
            {
                _availableJump--;
            }
        }
        //apply downward force for snappier jump feeling
        if (Input.GetButtonUp("Jump") || _rb.linearVelocityY < _jumpVelocityFallOff)
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
            Vector3 localScale = transform.localScale;
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
            
            MakeDashDust(DashDustDirectionX(), yRaw);

            StartCoroutine(ExecuteDash());

        }
    }
    private IEnumerator ExecuteDash()
    {
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        //disable gravity for total straight dash movement
        _rb.gravityScale = 0f;
        //diagonal cases
        if (yRaw != 0 && xRaw != 0)
        {
            _rb.linearVelocity = new Vector2(transform.localScale.x * _dashingPower * _dashNormalizer, yRaw * _dashingPower * _dashNormalizer);
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
        //tr.emitting = true;

        yield return new WaitForSeconds(dashingTime * _dashNormalizer);
        //tr.emitting = false;
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
    private void WallParticle()
    {
        var main = slideParticle.main;

        if (IsWalled() && !IsGrounded())
        {
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }
    private void MakeDust()
    {
        groundParticle.Play();
    }
    private void MakeDashDust(float xRaw, float yRaw)
    {
        var velocityDir = dashParticle.velocityOverLifetime;
        if (xRaw != 0 && yRaw != 0)
        {
            velocityDir.x = new ParticleSystem.MinMaxCurve(xRaw * _dashDustSpeed * 0.5f * _dashNormalizer, xRaw * _dashDustSpeed * _dashNormalizer);
            velocityDir.y = new ParticleSystem.MinMaxCurve(yRaw * _dashDustSpeed * 0.5f * _dashNormalizer, yRaw * _dashDustSpeed * _dashNormalizer);
        }
        else
        {
            velocityDir.x = new ParticleSystem.MinMaxCurve(xRaw * _dashDustSpeed * 0.5f, xRaw * _dashDustSpeed);
            velocityDir.y = new ParticleSystem.MinMaxCurve(yRaw * _dashDustSpeed * 0.5f, yRaw * _dashDustSpeed);
        }
        dashParticle.Play();
    }
    private float DashDustDirectionX()
    {
        return xRaw != 0 ? xRaw : _isFacingRight ? 1 : -1;
    }
    private void Freeze()
    {
        _pendingFreezeDuration = _freezeDuration;
    }
    private IEnumerator ExecuteFreeze()
    {
        
        _isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(_freezeDuration);
        Time.timeScale = original;
        _pendingFreezeDuration = 0;
        _isFrozen = false;
    }
}
