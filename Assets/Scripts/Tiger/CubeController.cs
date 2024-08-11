using UnityEngine;
using System.Collections;
using Unity.Netcode;


//호랑이 공격 스크립트
public class CubeController : NetworkBehaviour
{

    public GameObject playerHead;
    private string playerState; 
    private bool isActive = false; 
    private Renderer renderer;

    PlayerMovement _playerMovement;

    public bool IsActive
    {
        get { return isActive; }
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        playerState = playerHead.GetComponent<PlayerMovement>().playerState; //serverRpc로 고쳐야함.
        _playerMovement = playerHead.GetComponent<PlayerMovement>();
        // if(playerState == "Tiger"){
        //     SetCubeActive(false);
        // }
        SetCubeActive(false);
        
    }

    void Update()
    {
        if(!IsOwner){
            return;
        }


        if (Input.GetButtonDown("Interaction") && (playerState == "Tiger"||_playerMovement.isAwaken.Value))
        {
            Debug.Log("Attack!");
            StartCoroutine(ActivateAndDeactivateCube());
        }
    }

    IEnumerator ActivateAndDeactivateCube()
    {
        if (!isActive)
        {
            isActive = true;
            SetCubeActive(true);

            yield return new WaitForSeconds(2f);

            SetCubeActive(false);
            isActive = false;
        }
    }

    void SetCubeActive(bool active)
    {
        if (renderer != null)
        {
            renderer.enabled = active;
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isActive && playerState=="Tiger" && collision.gameObject.CompareTag("NPC")) //호랑이가 NPC 사냥
        {
            if (playerHead.GetComponent<PlayerMovement>() != null)
            {
                playerHead.GetComponent<PlayerMovement>().HuntFailure();
            }
        }
        else if(isActive && playerState == "Tiger" && collision.gameObject.CompareTag("Player")) //호랑이가 여우 사냥
        {
            Debug.Log("Tiger Hunts!");
        }
        else if(isActive && _playerMovement.isAwaken.Value && collision.gameObject.CompareTag("Player")) //여우(이미호)가 호랑이 사냥
        {
            // if(collision.gameObject.GetComponent<PlayerMovement>().playerState == "Tiger")
                Debug.Log("Fox Hunts!!!");
            
        }
    }
}
