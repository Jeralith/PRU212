using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Instructions : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement playerMovement;
    public FlashEffect flashEffect;
    [SerializeField, Range(0f, 1f)] private float greyValue;
    #region GameObjects
    [Header("Game Objects")]
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject leftBtn;
    [SerializeField] private GameObject rightBtn;
    [SerializeField] private GameObject shiftBtn;
    [SerializeField] private GameObject upDashBtn;
    [SerializeField] private GameObject rightDashBtn;
    [SerializeField] private GameObject spaceBtn1;
    [SerializeField] private GameObject spaceBtn2;
    [SerializeField] private GameObject trial1;
    [SerializeField] private GameObject trial2;
    [SerializeField] private GameObject _backgroundMusic;
    
    #endregion
    #region Sprite Renderers
    private SpriteRenderer upButtonRenderer;
    private SpriteRenderer downButtonRenderer;
    private SpriteRenderer leftButtonRenderer;
    private SpriteRenderer rightButtonRenderer;
    private SpriteRenderer space1ButtonRenderer;
    private SpriteRenderer space2ButtonRenderer;
    private SpriteRenderer shiftButtonRenderer;
    private SpriteRenderer upDashButtonRenderer;
    private SpriteRenderer rightDashButtonRenderer;
    private SpriteRenderer trialRenderer1;
    private SpriteRenderer trialRenderer2;
    #endregion
    [ColorUsage(true, true)]
    [SerializeField] private Color buttonFlashColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color blockFlashColor;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip unlocked;
    [SerializeField] private AudioClip blockBreak;
    private bool hasPlayedUnlockedAudio = false;
    private AudioLowPassFilter _lowPass;
    private bool _isFrozen;
    public float length;
    public CameraShake cameraShake;
    private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _freezeDuration;
    private bool leftPressed = false, rightPressed = false, jumpPressed = false, doubleJumpPressed = false, dashPressed = false;

    
    void Start()
    {
        SetupButton(leftBtn, greyValue);
        SetupButton(rightBtn, greyValue);
        SetupButton(shiftBtn, greyValue);
        SetupButton(upDashBtn, greyValue);
        SetupButton(rightDashBtn, greyValue);
        SetupButton(spaceBtn1, greyValue);
        SetupButton(spaceBtn2, greyValue);
        SetupButton(trial1, greyValue);
        SetupButton(trial2, greyValue);
        length = blockBreak.length;
        block.SetActive(true);
        playerMovement = player.GetComponent<PlayerMovement>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _lowPass = _backgroundMusic.GetComponent<AudioLowPassFilter>();
        _freezeDuration = unlocked.length;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && !leftPressed)
        {
            ActivateButton(leftBtn);
            leftPressed = true;
        }
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && !rightPressed)
        {
            ActivateButton(rightBtn);
            rightPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !jumpPressed)
        {
            ActivateButton(spaceBtn1);
            ActivateButton(trial1);
            jumpPressed = true;
        }
        if (playerMovement._hasDoubleJumped && !doubleJumpPressed)
        {
            ActivateButton(spaceBtn2);
            ActivateButton(trial2);
            doubleJumpPressed = true;
        }
        if (playerMovement._isDashing
        && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && !dashPressed)
        {
            ActivateButton(upDashBtn);
            ActivateButton(rightDashBtn);
            ActivateButton(shiftBtn);
            dashPressed = true;
        }
        if (leftPressed && rightPressed && jumpPressed && doubleJumpPressed && dashPressed && !hasPlayedUnlockedAudio)
        {
            PlaySFXClip(unlocked);
            hasPlayedUnlockedAudio = true;
            StartCoroutine(ExecuteFreeze(0.2f, unlocked.length));
            StartCoroutine(DestroyBlock());

        }
    }

    private void ActivateButton(GameObject gameObject)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null)
        {
            flashEffect = gameObject.GetComponent<FlashEffect>();
            flashEffect.CallFlash(1f, 0.2f, buttonFlashColor);
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            PlaySFXClip(buttonClick);
        }
    }
    private void SetupButton(GameObject gameObject, float greyValue)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(greyValue, greyValue, greyValue);
    }
    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }
    private IEnumerator ExecuteFreeze(float timeScale, float duration)
    {
        _isFrozen = true;
        _lowPass.cutoffFrequency = 300f;
        var original = playerMovement.timeScale;
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = original;
        _lowPass.cutoffFrequency = 22000f;
        _isFrozen = false;
    }
    private IEnumerator DestroyBlock()
    {
        yield return new WaitForSecondsRealtime(unlocked.length);
        PlaySFXClip(blockBreak);
        flashEffect = block.GetComponentInChildren<FlashEffect>();
        flashEffect.CallFlash(1f, 0.2f, blockFlashColor);
        if (cameraShake != null) CameraShake.instance.Shake(_impulseSource);
        block.GetComponentInChildren<TilemapRenderer>().material.color = Color.clear;
        yield return new WaitForSecondsRealtime(0.2f);
        block.SetActive(false);
    }
}
