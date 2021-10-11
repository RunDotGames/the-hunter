using System;
using UnityEngine;

namespace RDG.TheHunter {

  [Serializable]
  public class TheHuntConfig {
    public int daemonActionCount;
    public int daemonPriority;
    public float daemonMoveDuration;
    
    public PlayerBeh playerPrefab;
    public DaemonBeh daemonPrefab;

  }
  
  [CreateAssetMenu(menuName = "RDG/The Hunter/The Hunter Scene")]
  public class HuntSceneSo : ScriptableObject{
    public TheHuntConfig config;
    public GameGridSo grid;
    public ActionControllerSo actionControl;
    public IndicationSo indication;
    
    private PlayerBeh player;
    private DaemonBeh daemon;
    
    public void Begin(Transform root) {
      var gridNode = new GameObject("grid");
      gridNode.transform.parent = root;
      gridNode.transform.localPosition = Vector3.zero;
      gridNode.transform.localRotation = Quaternion.identity;
      grid.Begin(gridNode.transform);
      
      indication.Begin(root);
      
      player = Instantiate(config.playerPrefab, root).GetComponent<PlayerBeh>();
      player.name = "player";
      player.Place(new Vector2Int(0, Mathf.CeilToInt(grid.Size.y / 2.0f)));
      
      daemon = Instantiate(config.daemonPrefab, root).GetComponent<DaemonBeh>();
      daemon.name = "daemon";
      daemon.Place(new Vector2Int(grid.Size.x-1, Mathf.CeilToInt(grid.Size.y / 2.0f)));
      
      actionControl.Begin();
    }

    public void Release() {
      grid.Release();
      actionControl.Release();
      indication.Release();
    }
  }
  
  
  
}
