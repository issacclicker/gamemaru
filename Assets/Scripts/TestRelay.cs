using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class TestRelay : MonoBehaviour
{

    public string TestCode;

    // public GameObject CreateBtnText;

    //비활성화 시킬 UI
    public GameObject[] UI_Objects;

    public GameObject TestCodeUI;
    private Text _testCodeUI;
    //joinCode 빼내기
    private string _joinCode;

    private void Awake(){
        _testCodeUI = TestCodeUI.GetComponent<Text>();
    }

    
    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //CreateRelay();
        
    }

    public async void CreateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            // CreateBtnText.GetComponent<Text>().text = joinCode;

            _joinCode = joinCode;
            DisableUIObjects();
            JoinCodeText_Change();

        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }
        
        
    }

    public async void JoinRelay(string joinCode){
        try{
            Debug.Log("Joining : "+joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            DisableUIObjects();

            NetworkManager.Singleton.StartClient();
        }catch(RelayServiceException e){
            Debug.Log(e);
        }
    }

    public void onClick(){
        JoinRelay(TestCode);
    }

    private void JoinCodeText_Change(){
        _testCodeUI.text = _joinCode;
    }

    private void DisableUIObjects(){
        for(int i = 0;i<UI_Objects.Length;i++){
            UI_Objects[i].SetActive(false);
        }
    }
}
