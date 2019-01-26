﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {
    public string Name { get; private set; }

    /// <summary>
    /// The score that has been earned by the player
    /// </summary>
    public int Score { get; private set; }

    // Use this for initialization
    [SerializeField]private float speed;
    [SerializeField]private float jumpSpeed;
    [SerializeField]private float gravity;


    [SerializeField] private float minThrowPower;
    [SerializeField] private float maxThrowPower;
    [SerializeField] private float powerToAddEachFrame;

    [SerializeField] private float powerThrowForward;
    [SerializeField] private float powerThrowUpper;
    
    
    [SerializeField] private GameObject model;
    /// <summary>
    /// The number of pillows carried by the player
    /// </summary
    private int numPillowHold;
    private List<Pillow> Pillows;
    private List<Pillow> Ammo;
    private float emPower;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool oldTriggerHeldPick;
    private bool oldTriggerHeldThrow;

    void Start()
    {
        numPillowHold = 0;
        controller = GetComponent<CharacterController>();
        Ammo = new List<Pillow>();
        Pillows = new List<Pillow>();
        emPower = 0.0f;
        // let the gameObject fall down
        gameObject.transform.position = new Vector3(0, 5, 0);
    }

    void FixedUpdate()
    {
        // all input interface
        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * speed;
            Vector3 facingrotation = Vector3.Normalize(moveDirection);
            if (facingrotation != Vector3.zero)         //This condition prevents from spamming "Look rotation viewing vector is zero" when not moving.
                model.transform.forward = facingrotation;
           // model.transform.LookAt(new Vector3(0,moveDirection.y,0));
//            float heading = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
//            transform.rotation = Quaternion.Euler(0f, 0f, heading - 90);
            
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        bool newTriggerHeldPick = Input.GetAxis("Pick") > 0f;
        if(!oldTriggerHeldPick == newTriggerHeldPick)
        {
            if (numPillowHold >= 2)
            {
                // exceed the number that one player can hold 
                ThrowDrop();
            }
            else
            {
                PickUp();
            }
        }
        oldTriggerHeldPick = newTriggerHeldPick;
        
        
        
        bool newTriggerHeldThrow = Input.GetAxis("Throw") > 0f;
        if (newTriggerHeldThrow != oldTriggerHeldThrow)
        {
            if (newTriggerHeldThrow && numPillowHold > 0)
            {
                // start to empower the throw
                if (Ammo.Count != 0)
                {
                    Ammo[0].ReadyToGo(model);
                    emPower = minThrowPower;
                }
            }
            
        }

        if (emPower > 0.1f)
        {
            emPower = emPower < maxThrowPower ? emPower + powerToAddEachFrame : emPower; 
        }

        if (oldTriggerHeldThrow && !newTriggerHeldThrow) {
            /// throw
            if (Ammo.Count!=0)
            {
                Ammo[0].Throw( model.transform.forward, model.transform.up, (emPower - 0.08f + 1) * powerThrowForward, (emPower + 1) * powerThrowUpper);
                emPower = 0.0f;
                oldTriggerHeldThrow = false;
                Ammo.RemoveAt(0);
                numPillowHold--;
            } 
          
        }
        else {
          oldTriggerHeldThrow = newTriggerHeldThrow;
        }
        
        // Apply gravity
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
    }
    public void ThrowDrop() {

    }

    public void PickUp() {
            for (int i = 0; i < Pillows.Count; i++) {
                var temp = Pillows[i];
                if (temp.currentState==PillowState.Idle) {
                    temp.Pick(gameObject);
                    Ammo.Add(temp);
                    numPillowHold++;
                    break;
                }
            }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pillow") {
            var temp = other.GetComponent<Pillow>();
            if (temp.currentState == PillowState.Throwed){
                // doing dmg TODO

            }else{
                // sign into queue for pick up
                if (temp.currentState == PillowState.Idle) { Pillows.Add(temp);}
            }
        }   
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pillow")
        {
            var temp = other.GetComponent<Pillow>();
            if (Pillows.Contains(temp)) { 
                // sign out queue
                Pillows.Remove(temp);
            }
        }
    }
}