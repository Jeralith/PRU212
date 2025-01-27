using System.Collections;
using System.ComponentModel.Design;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    #region Fundementals
    [Header("Fundementals")]
    private Rigidbody2D _rb;
    private Collider2D _collider;
    [SerializeField] Collider2D _groundCollider;
    private bool _isFacingRight = true;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _maxFallSpeed = -20f;
    [SerializeField] private float _maxFallSpeedMultiflier = 1.5f;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;
    public float timeScale = .9f;
    public bool active;
    private Vector2 _respawnPoint;
    public bool _isGrounded;
    public bool isWalled;

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
    private float _superDashTime = 0.2f;
    private Vector2 _dashDirection;
    private bool _dashButtonPressed;
    #endregion
    #region Jump
    [Space]
    [Header("Jump")]
    [SerializeField] private bool canDoubleJump;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    [SerializeField] private float _jumpBufferTimeCounter;
    [SerializeField] private float _coyoteTimeCounter;
    private float _fallMultiplier = 7f;
    [SerializeField] private float _jumpVelocityFallOff = 8f;
    [SerializeField] private int _extraJump = 1;
    private int _availableJump;
    private bool _jumpButtonPressed;
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
    [SerializeField] private float _wallJumpingLerp = 10f;
    #endregion
    #region Collisions
    [Space]
    [Header("Collisions")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private Transform _middleCheck;
    [SerializeField] private LayerMask _killable;
    [SerializeField] private Transform _cornerCheckLeft;
    [SerializeField] private Transform _cornerCheckRight;
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
    public FlashEffect flashEffect;
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
    #region Colors
    [Space]
    [Header("Colors")]
    [ColorUsage(true, true)]
    [SerializeField] private Color _refillColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _dashColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _doubleJumpColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color _deathColor;
    #endregion
    #endregion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _collider = GetComponent<Collider2D>();
        active = true;
        SetRespawnPoint(transform.position);
        Time.timeScale = timeScale;
    }
    private void Update() //update sẽ chạy mỗi frame
    {
        if (_isDashing || !active)
        {
            return;
        }
        xRaw = Input.GetAxisRaw("Horizontal"); // -1 0 1
        yRaw = Input.GetAxisRaw("Vertical");   // -1 0 1
        x = Input.GetAxis("Horizontal");       //controller, joystick, analog control => slide từ -1 => 1 e.g: -0.323
        

        JumpInput();
        DashInput();
        _isGrounded = IsGrounded();

    }
    private void FixedUpdate() //update mỗi số frame (2-3-4 frame) ít độc lập frame hơn => ít responsive hơn
    {
        if (_isDashing)
        {
            return;
        }
        HorizontalMovement();

        Jump();

        Dash();

        if (!_isWallJumping)
            Flip();


        WallSlide();

        WallJump();

        WallDust();
        _wasGrounded = IsGrounded();

    }
    #region Collision Check
    private bool IsGrounded() => Physics2D.OverlapCircle(_groundCheck.position, 0.25f, _groundLayer);

    private bool IsWalled() => Physics2D.OverlapCircle(_wallCheck.position, 0.1f, _wallLayer);

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Killable"))
        {
            flashEffect.CallFlash(0.5f, 0.2f, _deathColor);

        }
    }

    #endregion
    #region Basic Movement
    private void HorizontalMovement()
    {

        //if player is walljumping, use a slightly different movement mechanic
        if (!_isWallJumping || IsGrounded())
        {
            _rb.linearVelocity = new Vector2(x * _speed, _rb.linearVelocityY);
        }
        else if (!IsGrounded())
        {
            var wallJumpingVelocity = _rb.linearVelocity;
            var playerVelocity = new Vector2(x * _speed, _rb.linearVelocityY);
            _rb.linearVelocity = Vector2.Lerp(wallJumpingVelocity, playerVelocity, _wallJumpingLerp * Time.fixedDeltaTime);
        }
        if (!_wasGrounded && IsGrounded())
        {
            GroundDust();
            if (!_canDash || _availableJump <= 0)
            if (flashEffect != null)
                flashEffect.CallFlash(0.5f, 0.1f, _refillColor);
        }
        // if (xRaw == 0 && _rb.linearVelocityX != 0 && IsGrounded())
        // {
        //     Vector2 force = Vector2.right * (_isFacingRight ? -1 : 1) * _decceleration;
        //     _rb.linearVelocityX += _decceleration;
        // }

    }
    private void JumpInput()
    {
        if (Input.GetButtonDown("Jump")) _jumpButtonPressed = true;
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
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        //also activate buffer is player is dashing
        if (_jumpButtonPressed)
        {
            _jumpBufferTimeCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimeCounter -= Time.fixedDeltaTime;
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
            _coyoteTimeCounter = 0f;
            _jumpButtonPressed = false;
        }
        //double jump condition
        else if (_jumpButtonPressed && _coyoteTimeCounter <= 0f && _availableJump > 0 && canDoubleJump && _wallJumpingCounter <= 0f)
        {

            GroundDust();
            if (flashEffect != null) flashEffect.CallFlash(1f, 0.1f, _doubleJumpColor);
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed);
            _jumpBufferTimeCounter = 0f;
            if (!IsWalled())
            {
                _availableJump--;
            }
            _jumpButtonPressed = false;
        }
        //limit fall speed
        if (_rb.linearVelocityY < FallSpeed())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, FallSpeed());
        }
        //apply downward force for snappier jump feeling
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (!Input.GetButton("Jump") || _rb.linearVelocityY > _jumpVelocityFallOff)
        {
            _rb.linearVelocity += _fallMultiplier * Physics2D.gravity.y * Vector2.up * Time.fixedDeltaTime;
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
    private void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) _dashButtonPressed = true;
    }
    private void Dash()
    {
        //dash is refilled when player touches ground
        if (IsGrounded())
        {
            _canDash = true;
        }
        if (_dashButtonPressed && _canDash)
        {
            StartCoroutine(ExecuteDash());
            _dashButtonPressed = false;
        }
    }
    private IEnumerator ExecuteDash()
    {
        if (_freezeFrame)
        {
            _isFrozen = true;
            var original = Time.timeScale;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(_freezeDuration);
            Time.timeScale = original;
            _pendingFreezeDuration = 0;
            _isFrozen = false;
        }

        if (_shockwaveManager != null) _shockwaveManager.CallShockwave();
        if (flashEffect != null) flashEffect.CallFlash(1f, 0.3f, _dashColor);
        if (cameraShake != null) CameraShake.instance.Shake(_impulseSource);
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        //disable gravity for total straight dash movement
        _rb.gravityScale = 0f;

        _dashDirection = new Vector2(xRaw, yRaw).normalized * _dashingPower;
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = new Vector2(transform.localScale.x * _dashingPower, 0);
        }
        _rb.linearVelocity = _dashDirection;

        float yRec = yRaw;
        // //draw trial
        if (tr != null) tr.emitting = true;
        DashDust(DashDustDirectionX(), yRaw);


        yield return new WaitForSeconds(_dashingTime * _dashNormalizer);
        if (tr != null) tr.emitting = false;
        if (yRec > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocityX, 0f);
        }
        _rb.gravityScale = originalGravity;
        //rb.linearVelocity = new Vector2(0f, 0f);
        _isDashing = false;
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
            _wallJumpingCounter -= Time.fixedDeltaTime;
        }

        if (_jumpBufferTimeCounter > 0f && _wallJumpingCounter >= 0f)
        {
            _isWallJumping = true;
            _rb.linearVelocity = new Vector2(0f, 0f);
            Vector2 force = Vector2.right * _speed * 1.5f * _wallJumpingDirection + Vector2.up * _jumpSpeed;
            DisableMovement(0.1f);
            _rb.linearVelocity += force;

            _wallJumpingCounter = 0f;
            _jumpBufferTimeCounter = 0f;
            if (_wallJumpingDirection != transform.localScale.x)
            {
                Vector3 localScale = transform.localScale;
                _isFacingRight = !_isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            if (IsGrounded() || IsWalled())
            {
                _isWallJumping = false;
            }
            Invoke(nameof(StopWallJumping), 0.3f);

            _jumpButtonPressed = false;
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
        if (slideParticle == null) return;
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
        if (slideParticle == null) return;
        groundParticle.Play();
    }
    private void DashDust(float xRaw, float yRaw)
    {
        if (slideParticle == null) return;
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
    #region Death
    private void MiniJump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocityX, _jumpSpeed / 2);
    }
    public void Die()
    {
        active = false;
        _collider.enabled = false;
        _groundCollider.GetComponent<Collider2D>().enabled = false;
        MiniJump();
        StartCoroutine(Respawn());
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        transform.position = _respawnPoint;
        active = true;
        _collider.enabled = true;
        _groundCollider.GetComponent<Collider2D>().enabled = true;
    }
    public void SetRespawnPoint(Vector2 position)
    {
        _respawnPoint = position;
    }
    #endregion
    public void CornerCorrection(Collider2D other)
    {
        // Bounds bounds = other.bounds;
        // float dir = Mathf.Min(this.transform.position.x - bounds.max.x, this.transform.position.x - bounds.min.x);
        // transform.position += new Vector3(dir, 0f);
    }
    private void DisableMovement(float seconds)
    {
        StartCoroutine(Disable(seconds));
    }
    private IEnumerator Disable(float seconds)
    {
        active = false;
        yield return new WaitForSeconds(seconds);
        active = true;
    }
}
