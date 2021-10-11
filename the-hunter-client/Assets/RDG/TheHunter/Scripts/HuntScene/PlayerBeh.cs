using System;
using System.Collections.Generic;
using RDG.UnityInput;
using RDG.UnityUtil;
using UnityEngine;

namespace RDG.TheHunter {
  public class PlayerBeh : MonoBehaviour{
    public ActionControllerSo actionControl;
    public GameGridSo grid;
    public PlayerSo player;
    public KeyActionsSo actions;
    public IndicationSo indication;
    
    public Guid Guid { get; } = Guid.NewGuid();

    private ActionTaker actionTaker;
    private KeyActionStack keyActionStack;
    private KeyAction direction = KeyAction.None;
    private bool placed;
    private List<GridNode> adjacent;
    
    public void Place(Vector2Int position) {
      var playerPoint = grid.GetNode(position);
      var trans = transform;
      trans.position = playerPoint.Object.transform.position;
      trans.forward = Vector3.right;
      grid.PlaceItemAt(Guid, playerPoint.Guid);
      placed = true;
      
      actionTaker = actionControl.NewActor(player.config.playerActionCount,  player.config.playerPriority);
      actionTaker.OnActionsDone += HandleGameActionsDone;
      actionTaker.OnActionsReady += HandleGameActionsReady;
      
      keyActionStack = new KeyActionStack(actions, new []{KeyAction.MoveLeft, KeyAction.MoveRight});
      keyActionStack.OnStackTopChange += HandleInput;
      adjacent = grid.GetAdjacent(grid.WhereIsItem(Guid));

      player.OnPlaceRequested += HandleOnPlaceRequested;
      player.OnMoveRequested += HandleOnMoveRequested;
    }
    private void HandleOnMoveRequested() {
      if (indication.GetForwardNodeId() == Guid.Empty || !player.IsInteractable || !placed) {
        return;
      }
      var forwardNode = grid.GetNode(indication.GetForwardNodeId());
      grid.PlaceItemAt(Guid, forwardNode.Guid);
      var trans = transform;
      trans.position = forwardNode.Object.transform.position;
      adjacent = grid.GetAdjacent(grid.WhereIsItem(Guid));
      actionTaker.TakeAction();  
    }
    
    private void HandleOnPlaceRequested() {
      actionTaker.TakeAction();
    }
    
    public void OnDestroy() {
      actionTaker?.Release();
      keyActionStack?.Release();
      player.Release();
    }
    
    private void HandleGameActionsReady() {
      player.SetInteractable(true);
    }
    
    private void HandleGameActionsDone() {
      player.SetInteractable(false);
    }
    
    
    private void HandleInput(KeyAction action) {
      direction = action;
    }

    public void Update() {
      UpdateRotation();
      if (!player.IsInteractable) {
        return;
      }
      
      foreach (var node in adjacent) {
        var myTransform = transform;
        var towards = node.Object.transform.position - myTransform.position;
        var prod = Vector3.Dot(towards.normalized,  myTransform.forward.normalized);
        if (!(prod > .5)) {
          continue;
        }
        
        indication.ForwardShow(node.Guid);
      }
    }

    private void UpdateRotation() {
      if (direction == KeyAction.None) {
        return;
      }
      var magnitude = direction == KeyAction.MoveLeft ? -1.0f : 1.0f;
      transform.Rotate(transform.up, player.config.playerRotationSpeed *  magnitude * Time.deltaTime);
    }
  }
}
