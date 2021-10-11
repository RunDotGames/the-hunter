using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.TheHunter {
  public class DaemonBeh : MonoBehaviour{
    public ActionControllerSo actionControl;
    public GameGridSo grid;
    public HuntSceneSo huntScene;
    
    public Guid MyGuid { get; } = Guid.NewGuid();

    private ActionTaker actionTaker;
    private readonly HashSet<Guid> trail  =  new HashSet<Guid>();

    public void Place(Vector2Int position) {
      var playerPoint = grid.GetNode(position);
      var trans = transform;
      trans.position = playerPoint.Object.transform.position;
      trans.forward = Vector3.right;
      grid.PlaceItemAt(MyGuid, playerPoint.Guid);
      
      actionTaker = actionControl.NewActor(huntScene.config.daemonActionCount,  huntScene.config.daemonPriority);
      actionTaker.OnActionsDone += HandleGameActionsDone;
      actionTaker.OnActionsReady += HandleGameActionsReady;
    }
    
    public void OnDestroy() {
      actionTaker?.Release();
    }
    
    private void HandleGameActionsReady() {
      HandleWonder();
    }
    
    private void HandleGameActionsDone() {
      trail.Clear();
    }

    private void HandleWonder() {
      if (actionTaker.ActionCountAvailable <= 0) {
        return;
      }

      var at = grid.WhereIsItem(MyGuid);
      trail.Add(at);
      var adjacent = grid.GetAdjacent(at);
      var eligible = new List<Guid>();
      foreach (var option in adjacent) {
        if (trail.Contains(option.Guid)) {
          continue;
        }
        eligible.Add(option.Guid);
      }
      if (eligible.Count <= 0) {
        // cant go anywhere, we are done
        actionTaker.TakeAction();
        HandleWonder();
        return;
      }
      var target = eligible[UnityEngine.Random.Range(0, eligible.Count)];
      StartCoroutine(WonderRoutine(target));
    }
    
    private IEnumerator<YieldInstruction> WonderRoutine(Guid target) {
      trail.Add(target);
      var startPosition = transform.position;
      var endPosition = grid.GetNode(target).Object.transform.position;
      transform.forward = endPosition - startPosition;
      var duration = 0.0f;
      while (true) {
        yield return new WaitForEndOfFrame();
        duration += Time.deltaTime;
        if (duration >= huntScene.config.daemonMoveDuration) {
          break;
        }
        var percent = duration / huntScene.config.daemonMoveDuration;
        transform.position = Vector3.Lerp(startPosition, endPosition, percent);
      }
      transform.position = endPosition;
      grid.PlaceItemAt(MyGuid, target);
      actionTaker.TakeAction();
      HandleWonder();
    }


  }
}
