using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawner : MonoBehaviour
{
	[SerializeField] private List<SpawnerItem> spawnerList;

	private void Awake()
	{
		SpawnScenarioEvent.SetTargetEvent += Spawn;
	}
	public void Spawn()
	{
		if (NetworkManager.Singleton.IsServer) {
			foreach (var item in spawnerList) {
				var instance = Instantiate(item.prefab, item.transform.position, item.transform.rotation);
				instance.GetComponent<NetworkObject>()?.Spawn();
			}
		}
	}
}

public static class SpawnScenarioEvent
{
	public delegate void SpawnEvent();
	public static event SpawnEvent SetTargetEvent;

	public static void TriggerSpawnEvent()
	{
		SetTargetEvent?.Invoke();
	}
}

[System.Serializable]
public struct SpawnerItem
{
	public Transform transform;
	public GameObject prefab;
}