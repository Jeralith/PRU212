using System.Runtime.CompilerServices;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement playerMovement;
    public FlashEffect flashEffect;
    [SerializeField, Range(0f, 1f)] private float greyValue;
    [Header("UI Button Images")]

    [SerializeField] private GameObject upBtn;
    [SerializeField] private GameObject downBtn;
    [SerializeField] private GameObject leftBtn;
    [SerializeField] private GameObject rightBtn;
    [SerializeField] private GameObject shiftBtn;
    [SerializeField] private GameObject upDashBtn;
    [SerializeField] private GameObject rightDashBtn;
    [SerializeField] private GameObject spaceBtn1;
    [SerializeField] private GameObject spaceBtn2;
    [SerializeField] private GameObject trial1;
    [SerializeField] private GameObject trial2;


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
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor;

    private bool upPressed, downPressed, leftPressed, rightPressed, jumpPressed, doubleJumpPressed, dashPressed;
    void Start()
    {
        SetupButton(upBtn, greyValue);
        SetupButton(downBtn, greyValue);
        SetupButton(leftBtn, greyValue);
        SetupButton(rightBtn, greyValue);
        SetupButton(shiftBtn, greyValue);
        SetupButton(upDashBtn, greyValue);
        SetupButton(rightDashBtn, greyValue);
        SetupButton(spaceBtn1, greyValue);
        SetupButton(spaceBtn2, greyValue);
        SetupButton(trial1, greyValue);
        SetupButton(trial2, greyValue);

        upPressed = false;
        downPressed = false;
        jumpPressed = false;
        doubleJumpPressed = false;
        dashPressed = false;

        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !upPressed)
        {
            ActivateButton(upBtn);
            upPressed = true;
        }

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !downPressed)
        {
           ActivateButton(downBtn);
            downPressed = true;
        }

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
    }

    private void ActivateButton(GameObject gameObject) {
        if(gameObject.GetComponent<SpriteRenderer>() != null) {
            flashEffect = gameObject.GetComponent<FlashEffect>();
            flashEffect.CallFlash(1f, 0.2f, flashColor);
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    private void SetupButton(GameObject gameObject, float greyValue)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(greyValue, greyValue, greyValue);
    }
}
