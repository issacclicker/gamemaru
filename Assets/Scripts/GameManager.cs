using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum State {
        NotReady,
        Ready,
    }

    private State state;
    private bool isLocalPlayerReady;

    private void Awake(){
        state = State.NotReady;
    }

}
