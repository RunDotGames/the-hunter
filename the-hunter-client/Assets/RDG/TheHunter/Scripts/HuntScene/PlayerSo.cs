using System;
using UnityEngine;

namespace RDG.TheHunter {

  [Serializable]
  public class PlayerConfig {
    public int playerActionCount;
    public int playerPriority;
    public float playerRotationSpeed;
    
  }
  
  [CreateAssetMenu(menuName = "RDG/The Hunter/Player")]
  public class PlayerSo: ScriptableObject {
    public PlayerConfig config;

    public event Action OnMoveRequested;
    public event Action OnPlaceRequested;
    public event Action<bool> OnInteractableChange;

    private bool interactable;
    
    
    public void Release() {
      OnMoveRequested = null;
      OnPlaceRequested = null;
    }

    public void RequestMove() {
      OnMoveRequested?.Invoke();
    }

    public void RequestPlace() {
      OnPlaceRequested?.Invoke();
    }

    public bool IsInteractable => interactable;

    public void SetInteractable(bool isInteractable) {
      interactable = isInteractable;
      OnInteractableChange?.Invoke(isInteractable);
    }
  }
}
