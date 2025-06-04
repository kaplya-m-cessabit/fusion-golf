using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
	[field: SerializeField] public GameObject PlayerPrefab { get; private set; }

	public void PlayerJoined(PlayerRef player)
	{
		InterfaceManager.instance.PrintPlayerCount(Runner.SessionInfo.PlayerCount, Runner.SessionInfo.MaxPlayers);

		if (player == Runner.LocalPlayer)
		{
			StartCoroutine(SpawnRoutine());
		}
		else
		{
			InterfaceManager.instance.ChefIconShake();
		}

		IEnumerator SpawnRoutine()
		{
			yield return new WaitUntil(() => SpawnpointManager.Instance != null);
			yield return new WaitUntil(() => GameManager.instance != null);

			UIScreen.Focus(InterfaceManager.instance.playerSettingScreen);

			yield return new WaitForEndOfFrame();

			// force color availability to be evaluated
			GameManager.instance.ReservedPlayerVisualsChanged();

			// Wait until the client has selected their nickname/visual before giving them an avatar
			yield return new WaitUntil(() => UIScreen.activeScreen == InterfaceManager.instance.gameplayHUD);

			if (SpawnpointManager.GetSpawnpoint(out Vector3 location, out Quaternion orientation))
			{
				Debug.Log("Spawning player");
				Runner.SpawnAsync(
					prefab: PlayerPrefab,
					position: location,
					rotation: orientation,
					inputAuthority: player,
					onCompleted: (res) => { if (res.IsSpawned) { Runner.SetPlayerObject(Runner.LocalPlayer, res.Object); } }
				);
			}
			else
			{
				Debug.LogWarning("Unable to spawn player");
			}
		}
	}

	public void PlayerLeft(PlayerRef player)
	{
		InterfaceManager.instance.PrintPlayerCount(Runner.SessionInfo.PlayerCount, Runner.SessionInfo.MaxPlayers);
		GameManager.instance.ReservedPlayerVisualsChanged();
	}
}