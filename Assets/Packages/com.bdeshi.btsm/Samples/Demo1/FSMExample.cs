using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BDeshi.BTSM;

public class FSMExample : MonoBehaviour
{
    FSMRunner runner;
    private void Awake()
    {
        runner = GetComponent<FSMRunner>();

        
        StateMachine<IState> fsm = new StateMachine<IState>();
    }
}
