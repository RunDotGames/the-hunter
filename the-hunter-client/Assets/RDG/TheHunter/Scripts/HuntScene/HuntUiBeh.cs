using RDG.UnityUI;
using UnityEngine;

namespace RDG.TheHunter {
  public class HuntUiBeh : MonoBehaviour {

    public PlayerSo player;
    public UiButtonBeh placeButton;
    public UiButtonBeh moveButton;
    public GameObject uiRoot;

    public void Start() {
      placeButton.OnClick += HandlePlace;
      moveButton.OnClick += HandleMove;
      player.OnInteractableChange += HandleInteractableChange;
      HandleInteractableChange(player.IsInteractable);
    }
    private void HandleInteractableChange(bool isInteractable) {
      placeButton.SetClickDisabled(!isInteractable);
      moveButton.SetClickDisabled(!isInteractable);
    }

    public void OnDestroy() {
      placeButton.OnClick -= HandlePlace;
      moveButton.OnClick -= HandleMove;
    }
    private void HandlePlace() {
      player.RequestPlace();
    }

    private void HandleMove() {
      player.RequestMove();
    }
  }
}
