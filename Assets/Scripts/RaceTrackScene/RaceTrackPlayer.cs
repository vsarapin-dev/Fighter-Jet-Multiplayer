using System.Collections.Generic;
using Michsky.UI.Shift;
using Mirror;
using UnityEngine;

namespace RaceTrackScene
{
    public class RaceTrackPlayer : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> fighterPlanesPrefabs = new List<GameObject>();
    
        private List<Transform> _fighterPlaneSpawnPositions = new List<Transform>();

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            gameObject.name = "LocalPlayer";
        }

        public override void OnStartLocalPlayer()
        {
            if (hasAuthority == false) return;
        
            int selectedIndex = PlayerPrefs.GetInt("SelectedFighterPlain", 0);
            DestroyBGMusicFromLobby();
            FindSpawnPositions();
            Transform positionToSpawn = _fighterPlaneSpawnPositions[selectedIndex];
            CmdSpawnFighterPlanePrefab(selectedIndex, positionToSpawn.position, positionToSpawn.rotation);
        }
    
        private void DestroyBGMusicFromLobby()
        {
            UIManagerBGMusic backgroundMusic = FindObjectOfType<UIManagerBGMusic>();
        
            if (backgroundMusic != null)
            {
                Destroy(backgroundMusic.gameObject);
            }
        }

        private void FindSpawnPositions()
        {
            if (_fighterPlaneSpawnPositions.Count > 0) return;
        
            GameObject spawnPositionsParent = GameObject.FindWithTag("SpawnPositions");
            foreach (Transform child in spawnPositionsParent.transform)
            {
                _fighterPlaneSpawnPositions.Add(child);
            }
        }

        [Command]
        private void CmdSpawnFighterPlanePrefab(int selectedIndex, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedPlane = Instantiate(
                fighterPlanesPrefabs[selectedIndex], 
                position, 
                rotation
            );
        
            NetworkServer.Spawn(spawnedPlane.gameObject, connectionToClient);
        }
    }
}
