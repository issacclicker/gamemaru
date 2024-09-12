using UnityEngine;
using System.Collections;
using Unity.Netcode;


//호랑이 공격 스크립트
public class CubeController : NetworkBehaviour
{

    public GameObject playerHead;
    private string playerState;
    private bool isActive = false;
    private bool isHunting = false;
    private Renderer renderer;
    private Tiger_animation tigerAnimation;

    PlayerMovement _playerMovement;

    GameObject PlayerCounterObject;
    private int PlayerCount;

    private int HuntedFoxCounter;

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

        PlayerCounterObject = GameObject.Find("PlayerCountText");
        PlayerCount = PlayerCounterObject.GetComponent<PlayerCounterNetwork>().playerCount.Value;
        Debug.Log("플레이어 수 : " + PlayerCount);
        HuntedFoxCounter = 0;

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }


        if (Input.GetButtonDown("Interaction") && (playerState == "Tiger" || _playerMovement.isAwaken.Value) && !isActive)
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

            //Tiger_animation
            Tiger_animation tigerAnimation = FindObjectOfType<Tiger_animation>();
            if (tigerAnimation != null)
            {
                tigerAnimation.PlayAttackAnimation();
            }

            SetCubeActive(true);

            yield return new WaitForSeconds(0.1f);

            SetCubeActive(false);
            isActive = false;
        }
    }

    void SetCubeActive(bool active)
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        Debug.Log(collision.gameObject.tag + " : " + collision.gameObject.name + " : " + collision.gameObject.GetComponent<NetworkObject>().OwnerClientId);

        if(IsHost && collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<NetworkObject>().OwnerClientId == 0)
        {
            Debug.Log("자기자신 공격");
            return;
        }

        if (isActive && playerState=="Tiger" && collision.gameObject.CompareTag("NPC")) //호랑이가 NPC 사냥
        {
            Debug.Log("호랑이가 NPC를 사냥!!");
            if (playerHead.GetComponent<PlayerMovement>() != null)
            {
                playerHead.GetComponent<PlayerMovement>().HuntFailure();
            }
        }
        else if (isActive && playerState == "Tiger" && collision.gameObject.CompareTag("Player")) //호랑이가 여우 사냥
        {
            if(collision.gameObject == this.gameObject || collision.gameObject.name == "TigerNet(Clone)")
            {
                Debug.Log("좆버그");
                return;
            }
            Debug.Log("호랑이가 여우를 사냥!!");

            OnTigerHuntsServerRpc(collision.gameObject);

            HuntedFoxCounter += 1;
            Debug.Log("여우 잡은 수 : " + HuntedFoxCounter);
        }
        else if (isActive && PlayerMovement.Instance.isAwaken.Value && collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerMovement>().isAwaken.Value) //여우(이미호)가 호랑이 사냥
        {

            if(collision.gameObject == this.gameObject)
            {
                Debug.Log("좆버그");
                return;
            }
            Debug.Log("여우가 호랑이를 사냥!!");

            OnSecFoxHuntsServerRpc(collision.gameObject);

        }
    }


    [ServerRpc]
    private void OnTigerHuntsServerRpc(NetworkObjectReference player)
    {
        if (player.TryGet(out var p))
        {
            Debug.Log("Tiger Hunts!");
            if (!p.GetComponent<PlayerMovement>().isAwaken.Value)
            {
                Debug.Log("호랑이가 여우를 사냥 성공함!");
                PlayerMovement.ProcessDieOnServer(p.GetComponent<PlayerMovement>());
                OnFoxDiesClientRpc();
            }

        }
    }

    [ServerRpc]
    private void OnSecFoxHuntsServerRpc(NetworkObjectReference player)
    {
        if (player.TryGet(out var p))
        {
            Debug.Log("Sec_Fox Hunts!");
            PlayerMovement.ProcessDieOnServer(p.GetComponent<PlayerMovement>());
            OnTigerDiesClientRpc(); 
        }
    }

    [ClientRpc]
    private void OnTigerDiesClientRpc()
    {
        Debug.Log("여우 승리1!!!");

        if(IsOwner)
        _playerMovement.GetComponent<PlayerMovement>()._uiManager.__EndGame__.GetComponent<EndGame>().GameOver();
    }


    [ClientRpc]
    private void OnFoxDiesClientRpc()
    {
        Debug.Log("죽은 여우 : " + HuntedFoxCounter + "/" + PlayerCounterObject.GetComponent<PlayerCounterNetwork>().playerCount.Value);
        HuntedFoxCounter+=1;

        if(HuntedFoxCounter>=PlayerCounterObject.GetComponent<PlayerCounterNetwork>().playerCount.Value-1&&IsOwner)
        {
            Debug.Log("호랑이 승리!!!!");
            _playerMovement.GetComponent<PlayerMovement>()._uiManager.__EndGame__.GetComponent<EndGame>().GameOver();
        }
    }
}
