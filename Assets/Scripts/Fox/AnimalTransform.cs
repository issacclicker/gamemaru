using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimalTransform : NetworkBehaviour
{
    //현재 모델에서 이미지를 변경하는 방식으로 구현했음
    //x키로 동물 이미지로 변경 -> 원래 이미지로 돌아오기
    //체력 소진시 원래 모델로 돌아감. 체력이 0 보다 클 때만 둔갑 가능. - HealthBar 스크립트와 연결되어 있음
    //현재는 2개의 이미지만 적용되어 있음
    //애니메이션은 아직 넘어온게 없어서 적용 못 함
    //Assets>AnimalTransform 폴더에 이미지 넣어둠
    //*AnimalTransform script 적용 후 Animal Models에 이미지 넣어두어야 제대로 작동함
    //*PlayerTestThirdPerson에 적용해서 그런지 원래의 이미지로 돌아갔을 때 사람과 함께 큐브가 적용된 이미지가 뜸

    // 동물 기몬 모델(여우,호랑이,고스트) 배열
    // 0 - fox
    // 1 - tiger
    // 2 - ghost
    public GameObject[] animalModelsForDefaultPlayer; // 동물 기몬 모델(여우,호랑이,고스트) 배열

    public string[] animalModelsForDefaultPlayerNames; // 동물 기몬 모델(여우,호랑이,고스트) 이름 배열


    public GameObject[] animalModelsForNetworks; // 동물 모델 NetworkObjects 적용된 Prefab

    public GameObject animalModel_Sec_Fox; // 이미호 동물 모델

    public string[] animalModelsNames; // 동물 모델 이름

    // private GameObject currentModel; // 현재 활성화된 모델
    // private GameObject originalModel; // 원래 사람 모델
    private bool isAnimal = false; // 현재 모델이 동물인지 여부

    int randomIndex_temp; //무작위 수 저장

    GameObject UIManagerObject; //UI매니저
    HealthBar _healthBar; //체력바
    PlayerMovement _playerMovement;

    private void Start()
    {
        if(!IsOwner)
        {
            return;
        }

        UIManagerObject = GameObject.Find("UIManager");
        _healthBar = UIManagerObject.GetComponent<HealthBar>();

        _playerMovement = GetComponent<PlayerMovement>();

        // DisableForiegnModelServerRpc(this.gameObject,"DummyMesh");
        // DisableForiegnModelClientRpc(this.gameObject,"DummyMesh");

        if(_playerMovement.playerStateSync.Value=="Fox")
        {
            PlayerNewModelSpawnServerRpc(this.gameObject,0);
        }
        else if(_playerMovement.playerStateSync.Value=="Tiger")
        {
            PlayerNewModelSpawnServerRpc(this.gameObject,1);
        }
        else
        {
            if(_playerMovement.playerState == "Fox")
            {
                PlayerNewModelSpawnServerRpc(this.gameObject,0);
            }
            else if(_playerMovement.playerState == "Tiger")
            {
                PlayerNewModelSpawnServerRpc(this.gameObject,1);
            }
        }

        // 현재 오브젝트를 기본 모델로 설정
        // _playerMovement.currentModel = new NetworkVariable<NetworkObjectReference>();
        // _playerMovement.currentModel.Value = gameObject.GetComponent<NetworkObject>();
        // _playerMovement.originalModel = gameObject;

    }

    private void Update()
    {

        if(!IsOwner){
            return;
        }


        if (_healthBar.health <0 && IsOwner){
            RevertToOriginalModel();
            Debug.Log("원래 모델로 변경");
        }
        // X키를 누르면 모델 변경 - 추후 키는 변경
        if (Input.GetKeyDown(KeyCode.X) && !_playerMovement.isAwaken.Value && PlayerMovement.Instance.playerState == "Fox" && IsOwner)
        {
            ChangeModel();
        }
        
    }

    public void ChangeModel()
    {

        // 원래 모델로 변경
        if (isAnimal)
        {
                //체력 닮기 비활성화
                _healthBar.isAnimal = false;

            NewModelDespawnServerRpc(this.gameObject,randomIndex_temp);

            isAnimal = false;
            Debug.Log("변신");
        }
        // 동물 모델로 변경
        else
        {
            // 둔갑이 가능한 상태일 때(체력이 0보다 클 때)만 동물 모델로 변경
            

            if (_healthBar != null && _healthBar.health > 0)
            {
                
                //체력 닮기 활성화
                _healthBar.isAnimal = true;

                // 랜덤으로 동물 모델
                // 1. 오리
                // 2. 양
                int randomIndex = Random.Range(0, animalModelsForNetworks.Length);
                randomIndex_temp = randomIndex;

                NewModelSpawnServerRpc(this.gameObject,randomIndex);

                isAnimal = true;
                Debug.Log("동물 모델로 변경");
            }
            else
            {
                Debug.Log("체력 부족 동물 모델로 변경 불가");
            }
        }

        

    }

    // 일정 시간이 경과했을 때 원래 모델로 돌아가기
    public void RevertToOriginalModel()
    {
        if (isAnimal && !_playerMovement.isAwaken.Value)
        {
            // this.transform.Find("DummyMesh").GetComponent<SkinnedMeshRenderer>().enabled = true; //DummyMesh 변경해야 할 수도.

            // AbleForiegnModelServerRpc(this.gameObject,"DummyMesh");
            // AbleForiegnModelClientRpc(this.gameObject,"DummyMesh");

            NewModelDespawnServerRpc(this.gameObject,randomIndex_temp);

            isAnimal = false;
            _healthBar.isAnimal = false;
            Debug.Log("원래 모델로 변경");
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void DisableForiegnModelServerRpc(NetworkObjectReference player,string meshName) // 메시 비활성화
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(meshName).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false; //"DummyMesh" 다른 거로 바꿔야 할 수도 있음
            Debug.Log("서버 알피시");
        }
    }

    [ServerRpc]
    private void AbleForiegnModelServerRpc(NetworkObjectReference player,string meshName) //메시 활성화
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(meshName).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true; //"DummyMesh" 다른 거로 바꿔야 할 수도 있음
            Debug.Log("서버 알피시");
        }
    }

    [ClientRpc]
    private void DisableForiegnModelClientRpc(NetworkObjectReference player,string meshName)
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(meshName).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
            Debug.Log("클라이언트 알피시");
        }
    }

    [ClientRpc]
    private void AbleForiegnModelClientRpc(NetworkObjectReference player,string meshName) //메시 활성화
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(meshName).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true; //"DummyMesh" 다른 거로 바꿔야 할 수도 있음
            Debug.Log("서버 알피시");
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void PlayerNewModelSpawnServerRpc(NetworkObjectReference player,int num)
    {
        if(player.TryGet(out var p))
        {
            GameObject newMd = Instantiate(animalModelsForDefaultPlayer[num]);

            var networkObject = newMd.GetComponent<NetworkObject>();
            networkObject.Spawn();
            networkObject.TrySetParent(p, worldPositionStays: false);
            // _playerMovement.currentModel.Value = networkObject;
        }

    }

    [ServerRpc (RequireOwnership = false)]
    public void PlayerNewModelDespawnServerRpc(NetworkObjectReference player,int num)
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(animalModelsForDefaultPlayerNames[num]).gameObject.GetComponent<NetworkObject>().Despawn();
        }

    }




    [ServerRpc]
    private void NewModelSpawnServerRpc(NetworkObjectReference player,int num)
    {
        if(player.TryGet(out var p))
        {
            if (_playerMovement.currentModel.Value.TryGet(out var exist) && exist.gameObject != _playerMovement.originalModel)
            {
                exist.Despawn();
                _playerMovement.currentModel.Value = default;
            }

            GameObject newMd = Instantiate(animalModelsForNetworks[num]);

            var networkObject = newMd.GetComponent<NetworkObject>();
            networkObject.Spawn();
            networkObject.TrySetParent(p, worldPositionStays: false);
            // _playerMovement.currentModel.Value = networkObject;
        }

    }

    [ServerRpc]
    private void NewModelDespawnServerRpc(NetworkObjectReference player,int num)
    {
        if(player.TryGet(out var p))
        {
            p.transform.Find(animalModelsNames[num]).gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SecondFoxModelSpawnServerRpc(NetworkObjectReference player)
    {
        if(player.TryGet(out var p))
        {
            if (_playerMovement.currentModel.Value.TryGet(out var exist))
                {
                    exist.Despawn();
                    // _playerMovement.currentModel.Value = default;
                }

                GameObject newMd = Instantiate(animalModel_Sec_Fox);

                var networkObject = newMd.GetComponent<NetworkObject>();
                networkObject.Spawn();
                networkObject.TrySetParent(p, worldPositionStays: false);
        }
    }

    public void ChangeModelToSecFox()
    {
        // 원래 모델 비활성화
                    // this.transform.Find("DummyMesh").GetComponent<SkinnedMeshRenderer>().enabled = false;

                    // DisableForiegnModelServerRpc(this.gameObject,"DummyMesh");
                    // DisableForiegnModelClientRpc(this.gameObject,"DummyMesh");

            //항상 가장 마지막에 이미호 모델 넣어야함.
            SecondFoxModelSpawnServerRpc(this.gameObject);
    }

}
