using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerBoatState BoatState { get; private set; }
    public PlayerFishState FishState { get; private set; }

    public Animator Animator {  get; private set; }
    [SerializeField] public GameObject exclamationMark;
    [SerializeField] public GameObject fishCaughtPanel;
    [SerializeField] public GameObject fishCaughtImage;
    [SerializeField] public GameObject fishCaughtNameText;

    public Rigidbody2D rb;
    public BoxCollider2D collider;
    public SpriteRenderer renderer;

    // Animator parameters
    public readonly string _horizontal = "Horizontal";
    public readonly string _vertical = "Vertical";
    public readonly string _lastHorizontal = "LastHorizontal";
    public readonly string _lastVertical = "LastVertical";

    public readonly Vector2 colliderSizeHorizontal = new Vector2(3.7462f, 1.2828f);
    public readonly Vector2 colliderSizeVertical = new Vector2(1.8523f, 2.5828f);
    public readonly Vector2 colliderOffsetHorizontal = new Vector2(0.2165f, -0.9760f);
    public readonly Vector2 colliderOffsetVertical = new Vector2(0.0119f, -0.5760f);

    public bool isOnShinySpot = false;

    public Vector2 moveDirection;
    public float moveSpeed;
    [SerializeField] public Sprite[] sprites;

    [SerializeField] public List<Fish> fishInventory;

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        BoatState = new PlayerBoatState(this, StateMachine);
        FishState = new PlayerFishState(this, StateMachine);
    }

    private void Start()
    {
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();

        moveSpeed = 4f;
        StateMachine.Initialize(BoatState);

        //TODO init
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Player collided with " + col.name);
        if (col.name == "ShinySpot") {
            isOnShinySpot = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.name == "ShinySpot") {
            isOnShinySpot = false;
        }
    }
}
