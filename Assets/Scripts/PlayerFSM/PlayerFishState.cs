using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerFishState : PlayerState
{
    // just setting this bool to true for now to test the animation
    bool nibble = false;
    float nibbleTimer;
    float nibbleWaitTime;
    float catchTimer;
    readonly float maxWaitTime = 6f; // can change this if we want it to be longer
    readonly float shinySpotMaxWaitTime = 3f;

    Vector3 exclamationLeftPos;

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

        float playerMaxWaitTime = player.isOnShinySpot ? shinySpotMaxWaitTime : maxWaitTime;
        nibbleWaitTime = Time.time + Random.Range(playerMaxWaitTime * 0.25f, playerMaxWaitTime);

        catchTimer = Time.time;
        exclamationLeftPos = player.exclamationMark.transform.position;
        Debug.Log(nibbleWaitTime);
    }

    public override void Exit()
    {
        player.Animator.SetBool("FishCaught", false);
        player.Animator.SetBool("IsFishing", false);
        player.exclamationMark.transform.position = exclamationLeftPos; // reset to original position, left is default
        base.Exit();
    }

    public override void Update()
    {
        // TODO: bug somethign is wrong with the catch timer (sometimes it triggers without nibble happening first)
        // Timer set ups
        nibbleTimer += Time.deltaTime; // timer for waiting when a fish will be on the line
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
            }
            else if (playerSprite.Contains("up"))
            {
                player.exclamationMark.transform.position = new Vector3(exclamationLeftPos.x + 0.3f, exclamationLeftPos.y + 0.4f, 0);
            }
            else if (playerSprite.Contains("down"))
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
                Fish fish = FishManager.GetRandomFish(Rarity.common).Item1;
                Debug.Log(fish.name);
                player.Animator.SetBool("FishCaught", true);
                this.startTime = Time.time;
                // player.getCoord
            }
            else
            {
                stateMachine.ChangeState(player.BoatState);
            }
        }

        // show off the fish that was just caught (the FishCaught boolean is still true so its still in the caught state)
        // Debug.Log(player.Animator.GetBool("FishCaught"));
        if (player.Animator.GetBool("FishCaught") && Time.time >= (this.startTime + 1.5f))
        {
            player.Animator.SetBool("FishCaught", false);
            stateMachine.ChangeState(player.BoatState);
        }

    }

}
