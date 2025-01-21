using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    #region Fundementals
    [Header("Fundementals")]
    private Rigidbody2D _rb;
    private bool _isFacingRight = true;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _maxFallSpeed = -20f;
    [SerializeField] private float _maxFallSpeedMultiflier = 1.5f;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;
    [SerializeField] private float _timeScale = .9f;
    #endregion
    #region Dash
    [Space]
    [Header("Dash")]
    private bool _canDash;
    private bool _isDashing;
    [SerializeField] private float _dashingPower = 24f;
    [SerializeField] private float _dashingTime = 0.2f;
    private float _dashNormalizer = 0.707f;
    [SerializeField] private bool _freezeFrame = true;
    private float _dashMomentum = 15f;
    private float _superDashCounter;
    private float _superDashTime = 0.2f;
    private Vector2 _dashDirection;
    #endregion
    #region Jump
    [Space]
    [Header("Jump")]
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferTimeCounter;
    private float _coyoteTimeCounter;
    private float _fallMultiplier = 7f;
    [SerializeField] private float _jumpVelocityFallOff = 8f;
    [SerializeField] private int _extraJump = 1;
    private int _availableJump;
    #endregion
    #region WallTech
    [Space]
    [Header("Wall Tech")]
    [SerializeField] private bool _isWallSliding;
    private bool _isWallJumping;
    [SerializeField] private float _wallSlidingSpeedMultiplier;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDirection;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    [SerializeField] private float _wallJumpingLerp = 10f;
    private Vector2 _wallJumpingPower = new Vector2(6f, 15f);
    #endregion
    #region Collisions
    [Space]
    [Header("Collisions")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform middleCheck;
    [SerializeField] private LayerMask transitionLayer;
    #endregion
    #region Misc
    [Space]
    [Header("Misc")]
    private float _freezeDuration = 0.05f;
    private bool _isFrozen;
    private float _pendingFreezeDuration = 0f;
    [SerializeField] private TrailRenderer tr;
    private bool _wasGrounded;
    [SerializeField] private ParticleSystem slideParticle;
    [SerializeField] private ParticleSystem groundParticle;
    [SerializeField] private ParticleSystem dashParticle;
    private float _dashDustSpeed = 5f;
    private float xRaw;
    private float yRaw;
    private float x;
    #endregion
    #region Camera
    [Space]
    [Header("Camera")]
    public ShockwaveManager _shockwaveManager;
    public CameraShake cameraShake;
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeStrength = 0.4f;
    private CinemachineImpulseSource _impulseSource;
    #endregion
    #endregion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void Update() //update sẽ chạy mỗi frame
    {
        if (_isDashing)
        {
            return;
        }
        if (_pendingFreezeDuration > 0 && !_isFrozen)
        {
            StartCoroutine(ExecuteFreeze());
        }
        xRaw = Input.GetAxisRaw("Horizontal"); // -1 0 1
        yRaw = Input.GetAxisRaw("Vertical");   // -1 0 1
        x = Input.GetAxis("Horizontal");       //controller, joystick, analog control => slide từ -1 => 1 e.g: -0.323
        Time.timeScale = _timeScale;
        HorizontalMovement();

        Jump();

        Dash();

        SuperDash();

        if (!_isWallJumping)
        {
            Flip();
        }

        WallSlide();

        WallJump();

        WallDust();

        _wasGrounded = IsGrounded();
    }
    private void FixedUpdate() //update mỗi số frame (2-3-4 frame) ít độc lập frame hơn => ít responsive hơn
    {
        if (_isDashing)
        {
            return;
        }
        
    }

    #region Collision Check
    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, 0.25f, groundLayer);

    private bool IsWalled() => Physics2D.OverlapCircle(wallCheck.position, 0.01f, wallLayer) 
                            && !Physics2D.OverlapCircle(middleCheck.position, 0.01f, wallLayer);

    private bool TransitionEnter() => Physics2D.OverlapCircle(wallCheck.position, 0.01f, transitionLayer);
    #endregion
    #region Basic Movement
    private void HorizontalMovement()
    {

        //if player is walljumping, use a slightly different movement mechanic
        if (!_isWallJumping)
        {
            _rb.linearVelocity = new Vector2(x * _speed, _rb.linearVelocityY);
        }
        else
        {
            var wallJumpingVelocity = new Vector2(_wallJumpingDirection * _speed, _rb.linearVelocityY);
            var playerVelocity = new Vector2(x * _speed, _rb.linearVelocityY);
            _rb.linearVelocity = Vector2.Lerp(wallJumpingVelocity, playerVelocity, _wallJumpingLerp * Time.fixedDeltaTime);
        }
        if (!_wasGrounded && IsGrounded())
        {
            GroundDust();
        }
        if (xRaw == 0 && IsGrounded())
        {
            _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, new Vector2(0, _rb.linearVelocityY), _decceleration * Time.fixedDeltaTime);
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
            GroundDust();
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            //if player touches wall, use wall jump instead 
            if (!IsWalled() && _coyoteTimeCounter <= 0f)
            {
                _availableJump--;
            }
        }
        //double jump condition
        else if (Input.GetButtonDown("Jump") && _coyoteTimeCounter <= 0f && _availableJump > 0 && canDoubleJump && _wallJumpingCounter <= 0f)
        {

            GroundDust();
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            if (!IsWalled())
            {
                _availableJump--;
            }
        }
        //limit fall speed
        if (_rb.linearVelocityY < FallSpeed())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, FallSpeed());
        }
        //apply downward force for snappier jump feeling
        if(_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        } else if (!Input.GetButton("Jump") || _rb.linearVelocityY > _jumpVelocityFallOff)
        {
            _rb.linearVelocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            _coyoteTimeCounter = 0f;
        }
    }
    private float FallSpeed() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? _maxFallSpeed * _maxFallSpeedMultiflier : _maxFallSpeed;
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
    #endregion
    #region Dash
    private void Dash()
    {
        //dash is refilled when player touches ground
        if (IsGrounded())
        {
            _canDash = true;
        }
        _superDashCounter -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            
            DashDust(DashDustDirectionX(), yRaw);
            StartCoroutine(ExecuteDash());

        }
    }
    private IEnumerator ExecuteDash()
    {
        if (_freezeFrame)
        {
            StartCoroutine(ExecuteFreeze());
        }

        _shockwaveManager.CallShockwave();
        CameraShake.instance.Shake(_impulseSource);
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        //disable gravity for total straight dash movement
        _rb.gravityScale = 0f;

        _dashDirection = new Vector2(xRaw, yRaw).normalized * _dashingPower;
        if (_dashDirection == Vector2.zero) {
            _dashDirection = new Vector2(transform.localScale.x * _dashingPower, 0);
        }
        _rb.linearVelocity = _dashDirection;
        
        float yRec = yRaw;
        // //draw trial
        tr.emitting = true;

        

        yield return new WaitForSeconds(_dashingTime * _dashNormalizer);
        tr.emitting = false;
        if (yRec > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0f);
        }
        _rb.gravityScale = originalGravity;
        //rb.linearVelocity = new Vector2(0f, 0f);
        _isDashing = false;
        _superDashCounter = _superDashTime;
    }
    #endregion
    #region Wall Tech
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && xRaw != 0 && _rb.linearVelocityY <= 0)
        {
            _rb.linearVelocityY *= _wallSlidingSpeedMultiplier;
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

        if ((Input.GetButtonDown("Jump") && _wallJumpingCounter > 0f) || _jumpBufferTimeCounter > 0f && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            _rb.linearVelocity = new Vector2(_wallJumpingDirection * _speed, _jumpSpeed);
            _wallJumpingCounter = 0f;
            _jumpBufferTimeCounter = 0f;
            if (_wallJumpingDirection != transform.localScale.x)
            {
                Vector3 localScale = transform.localScale;
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
    #endregion
    #region Particles
    private void WallDust()
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
    private void GroundDust()
    {
        groundParticle.Play();
    }
    private void DashDust(float xRaw, float yRaw)
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
        if (xRaw == 0 && yRaw != 0) return 0;
        return xRaw != 0 ? xRaw : _isFacingRight ? 1 : -1;
    }
    #endregion
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
