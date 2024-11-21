using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerFishState : PlayerState
{
    //types of minigames
    private string fishMinigameChase;
    private string fishMinigameMash;
    private string fishMinigameRhythm;
    private string fishMinigameHold;

    private Rigidbody2D catchingBarRB;
    private GameObject fishMinigameCanvas;
    private GameObject fishIcon;
    private GameObject catchingBar;

    private float catchMultiplier = 10f; //Higher means catch fish faster x
    private float catchingForce = 30000; //How much force to push the catchingbar up by
    private Fish currentFishOnLine;
    private bool beingCaught;

    private float catchPercentage = 0f; //0-100 how much you have caught the fish
    UnityEngine.UI.Slider catchProgressBar; 
    // just setting this bool to true for now to test the animation
    bool nibble = false;
    float nibbleTimer; // timer it takes to get a fish nibble
    float nibbleWaitTime; // Set time it takes to get a fish nibble calculated when entering this state; nibbleTimer ticks up until it reaches this time
    float catchTimer; // timer for how long player can wait to reach and catch the fish
    float maxWaitTime = 6f; // can change this if we want it to be longer

    Vector3 exclamationLeftPos;
    Vector3 fishCaughtPanelLeftPos;

    public PlayerFishState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        
        base.Enter();
        nibble = false;
        Debug.Log("fishing now");
        player.Animator.SetBool("IsFishing", true);
        nibbleTimer = Time.time;
        nibbleWaitTime = Time.time + Random.Range(maxWaitTime * 0.25f, maxWaitTime);
        catchTimer = Time.time;
        exclamationLeftPos = player.exclamationMark.transform.position;
        fishCaughtPanelLeftPos = player.fishCaughtPanel.transform.position;
        Debug.Log(nibbleWaitTime);

        fishMinigameChase = "/Player/PlayerCanvas/FishMinigame_Chase";
        fishMinigameMash = "/Player/PlayerCanvas/FishMinigame_Mash";
        fishMinigameRhythm = "/Player/PlayerCanvas/FishMinigame_Rhythm";
        fishMinigameHold = "/Player/PlayerCanvas/FishMinigame_HoldRelease";

        catchProgressBar = GameObject.Find(fishMinigameChase + "/CatchProgressBar").GetComponent<UnityEngine.UI.Slider>(); //The bar on the right that shows how much you have caught


    }

    public override void Exit()
    {
        player.Animator.SetBool("FishCaught", false);
        player.Animator.SetBool("IsFishing", false);
        player.fishCaughtPanel.SetActive(false);
        player.exclamationMark.transform.position = exclamationLeftPos; // reset to original position, left is default
        player.fishCaughtPanel.transform.position = fishCaughtPanelLeftPos;
        base.Exit();
    }

    public override void Update()
    {
        // TODO: bug somethign is wrong with the catch timer (sometimes it triggers without nibble happening first)
        // Timer set ups
        nibbleTimer += Time.deltaTime; // timer for waiting when a fish will be on the line
        // indicates a fish is on the line, exclamation mark shows up
        if (nibbleTimer >= nibbleWaitTime && !nibble)
        {
            nibble = true;
            player.exclamationMark.SetActive(true);
            this.startTime = Time.time;
            catchTimer = Time.time;
            string playerSprite = player.GetComponent<SpriteRenderer>().sprite.name;
            Debug.Log(playerSprite);
            if (playerSprite.Contains("right"))
            {
                player.exclamationMark.transform.position = new Vector3(exclamationLeftPos.x + 0.8f, exclamationLeftPos.y, 0);
            } else if (playerSprite.Contains("up"))
            {
                player.exclamationMark.transform.position = new Vector3(exclamationLeftPos.x + 0.3f, exclamationLeftPos.y + 0.4f, 0);
            } else if (playerSprite.Contains("down"))
            {
                player.exclamationMark.transform.position = new Vector3(exclamationLeftPos.x + 0.3f, exclamationLeftPos.y, 0);
            }
        }
        if (nibble)
        {
            catchTimer += Time.deltaTime; // timer starts when nibble happens, if player is too slow the fish gets away
        }
        
        // if player takes too long to catch fish, the fish gets away
        if (nibble & catchTimer > this.startTime + 2f)
        {
            nibble = false;
            player.Animator.SetBool("IsFishing", false);
            player.Animator.SetBool("FishCaught", false);
            player.exclamationMark.SetActive(false);
            stateMachine.ChangeState(player.BoatState);
        }
        // player presses F in time and should catch the fish that is on the line or cancel the fishing if there is no fish on the line
        if (Input.GetKeyDown(KeyCode.F))
        {
            player.Animator.SetBool("IsFishing", false);
            
            player.exclamationMark.SetActive(false);
            if (nibble)
            {
                player.Animator.SetBool("IsFishMinigame", true);
                currentFishOnLine = FishManager.GetRandomFish(Rarity.common).Item1; // need to reorganize, the game should be based on what fish is on the line
                this.startTime = Time.time;
                
            } else
            {
                stateMachine.ChangeState(player.BoatState);
            }
        }
        if (player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Catch"))
        {
            string playerSprite = player.GetComponent<SpriteRenderer>().sprite.name;
            if (playerSprite.Contains("right"))
            {
                player.fishCaughtPanel.transform.position = new Vector3(fishCaughtPanelLeftPos.x + 0.8f, fishCaughtPanelLeftPos.y, 0);
            }
            else if (playerSprite.Contains("up"))
            {
                player.fishCaughtPanel.transform.position = new Vector3(fishCaughtPanelLeftPos.x + 0.3f, fishCaughtPanelLeftPos.y + 0.4f, 0);
            }
            else if (playerSprite.Contains("down"))
            {
                player.fishCaughtPanel.transform.position = new Vector3(fishCaughtPanelLeftPos.x + 0.3f, fishCaughtPanelLeftPos.y, 0);
            }
            player.fishCaughtPanel.SetActive(true);
        }
        // show off the fish that was just caught (the FishCaught boolean is still true so its still in the caught state)
        // Debug.Log(player.Animator.GetBool("FishCaught"));
        if (player.Animator.GetBool("FishCaught") && Time.time >= (this.startTime + 1.5f))
        {
            player.Animator.SetBool("FishCaught", false);
            player.fishCaughtPanel.SetActive(false);
            stateMachine.ChangeState(player.BoatState);
        }

        // IN A FISHING MINIGAME
        if (player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Fishing"))
        {
            string fishMinigameString = fishMinigameChase; // TODO: later must make it depend on a condition to change the types of minigames
            fishMinigameCanvas = GameObject.Find(fishMinigameChase);
            fishIcon = GameObject.Find(fishMinigameChase + "/WaterBar/FishIcon");
            catchingBar = GameObject.Find(fishMinigameChase + "/WaterBar/CatchingBar");

            catchProgressBar = GameObject.Find(fishMinigameChase + "/CatchProgressBar").GetComponent<UnityEngine.UI.Slider>(); //The bar on the right that shows how much you have caught

            catchingBarRB = catchingBar.GetComponent<Rigidbody2D>(); //Get reference to the Rigidbody on the catchingbar
            fishMinigameCanvas.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            { 
                catchingBarRB.AddForce(Vector2.up * catchingForce * Time.deltaTime, ForceMode2D.Force); //Add force to lift the bar
            }
            
        }

        //If the fish is in our trigger box
        if (beingCaught && player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Fishing"))
        {
            catchPercentage += catchMultiplier * Time.deltaTime;
        }
        else
        {
            catchPercentage -= catchMultiplier * Time.deltaTime;
        }

        //Clamps our percentage between 0 and 100
        catchPercentage = Mathf.Clamp(catchPercentage, 0, 100);
        catchProgressBar.value = catchPercentage;
        if (catchPercentage >= 100)
        { //Fish is caught if percentage is full
            catchPercentage = 0;
            FishCaught();
        }

    }

    //Called when the catchpercentage hits 100
    public void FishCaught()
    {
        if (currentFishOnLine == null)
        { //This picks a new fish if the old one is lost by chance
            currentFishOnLine = FishManager.GetRandomFish(Rarity.common).Item1;
        }
        var tempSprite = Resources.Load<Sprite>($"FishSprites/{currentFishOnLine.speciesId}");
        player.fishCaughtImage.GetComponent<UnityEngine.UI.Image>().sprite = tempSprite;
        player.fishCaughtNameText.GetComponent<TMP_Text>().text = currentFishOnLine.name;

        Debug.Log(currentFishOnLine.name);
        player.Animator.SetBool("IsFishMinigame", false);
        player.Animator.SetBool("FishCaught", true);
        fishMinigameCanvas.SetActive(false); //Disable the fishing canvas
  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Fishing"))
        {
            if (other.CompareTag("CatchingBar") && !beingCaught)
            {
                beingCaught = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CatchingBar") && beingCaught)
        {
            beingCaught = false;
        }
    }
}
