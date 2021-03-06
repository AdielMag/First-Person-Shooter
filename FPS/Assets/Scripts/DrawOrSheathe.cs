﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOrSheathe : StateMachineBehaviour
{
    public bool draw;
    public int weaponNumTag;
    PlayerController pCon;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (draw)
        {
            PlayerController.instance.DrawWeapon(weaponNumTag);
            PlayerController.instance.weaponIsEquiped = true;
        }
        else
            PlayerController.instance.weaponIsEquiped = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!draw)
            PlayerController.instance.SheathWeapon(weaponNumTag);
        
    }

}
