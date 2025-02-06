using UnityEngine;

public class Instructions : MonoBehaviour
{
    public GameObject player;
    private PlayerMovement playerMovement;
    [SerializeField, Range(0f, 1f)] private float greyValue;
    [Header("UI Button Images")]
    [SerializeField] private SpriteRenderer upButtonRenderer;
    [SerializeField] private SpriteRenderer downButtonRenderer;
    [SerializeField] private SpriteRenderer leftButtonRenderer;
    [SerializeField] private SpriteRenderer rightButtonRenderer;
    [SerializeField] private SpriteRenderer space1ButtonRenderer;
    [SerializeField] private SpriteRenderer space2ButtonRenderer;
    [SerializeField] private SpriteRenderer shiftButtonRenderer;
    [SerializeField] private SpriteRenderer upDashButtonRenderer;
    [SerializeField] private SpriteRenderer rightDashButtonRenderer;
    [SerializeField] private SpriteRenderer trialRenderer1;
    [SerializeField] private SpriteRenderer trialRenderer2;

    // (Optional) Store each button's original color if you want to reset it later.
    private Color upOriginalColor;
    private Color downOriginalColor;
    private Color leftOriginalColor;
    private Color rightOriginalColor;
    private Color spaceOriginalColor;
    private bool _hasDashed;
    private bool _hasDoubleJumped;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        downButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        leftButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        rightButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        space1ButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        space2ButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        shiftButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        upDashButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        rightDashButtonRenderer.color = new Color(greyValue, greyValue, greyValue);
        trialRenderer1.color = new Color(greyValue, greyValue, greyValue);
        trialRenderer2.color = new Color(greyValue, greyValue, greyValue);
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (upButtonRenderer != null)
            {
                upButtonRenderer.color = Color.white;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (downButtonRenderer != null)
            {
                downButtonRenderer.color = Color.white;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (leftButtonRenderer != null)
            {
                leftButtonRenderer.color = Color.white;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (rightButtonRenderer != null)
            {
                rightButtonRenderer.color = Color.white;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (space1ButtonRenderer != null && trialRenderer1 != null)
            {
                space1ButtonRenderer.color = Color.white;
                trialRenderer1.color = Color.white;
            }
        }
        if (playerMovement._hasDoubleJumped)
        {
            if (space2ButtonRenderer != null && trialRenderer2 != null)
            {
                space2ButtonRenderer.color = Color.white;
                trialRenderer2.color = Color.white;
            }
        }
        if (playerMovement._isDashing 
        && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
        && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) {
            if (upDashButtonRenderer != null && rightDashButtonRenderer != null && shiftButtonRenderer != null)
            {
                upDashButtonRenderer.color = Color.white;
                rightDashButtonRenderer.color = Color.white;
                shiftButtonRenderer.color = Color.white;
            }
        }
    }
}
