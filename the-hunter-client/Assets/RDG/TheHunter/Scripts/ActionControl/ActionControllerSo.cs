using System.Collections.Generic;
using UnityEngine;

namespace RDG.TheHunter {

  [CreateAssetMenu(menuName = "RDG/The Hunter/GameController")]
  public class ActionControllerSo : ScriptableObject {

    private readonly List<ActionTakerImpl> actors = new List<ActionTakerImpl>();
    private int actorIndex = 0;
    private ActionTakerImpl last;
    private bool isRunning;

    public ActionTaker NewActor(int actionCount, int priority) {
      var actor = new ActionTakerImpl(actionCount, priority, HandleActorActionTaken, HandleActorReleased);
      actors.Add(actor);
      actors.Sort((first, second) => first.Priority - second.Priority);
      return actor;
    }

    private void HandleActorActionTaken(ActionTakerImpl actor) {
      if (actor.ActionCountAvailable > 0) {
        return;
      }
      
      ActivateNextActor();
    }

    private void HandleActorReleased(ActionTakerImpl actor) {
      var index = actors.IndexOf(actor);
      actors.Remove(actor);
      if (!isRunning) {
        return;
      }
        
      if (index > actorIndex) {
        actorIndex--;
      }
      if (index == actorIndex) {
        ActivateNextActor();
      }
    }

    public void Begin() {
      isRunning = true;
      actorIndex = -1;
      ActivateNextActor();
    }

    public void Release() {
      isRunning = false;
      actorIndex = -1;
      last = null;
      actors.Clear();
    }

    private void ActivateNextActor() {
      if (!isRunning) {
        return;
      }
      
      if (actors.Count == 0) {
        return;
      }

      last?.NotifyDone();
      actorIndex++;
      if (actorIndex >= actors.Count) {
        actorIndex = 0;
      }
      last = actors[actorIndex];
      last.NotifyReady();
    }


  }
}