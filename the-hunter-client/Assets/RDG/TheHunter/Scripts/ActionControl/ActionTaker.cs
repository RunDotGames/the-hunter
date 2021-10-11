using System;
using System.Threading.Tasks;

namespace RDG.TheHunter {
  public interface ActionTaker {
    public event Action OnActionsReady;
    public event Action OnActionsDone;
    public int ActionCountAvailable { get; }
    public int ActionCountAllocated { get; }
    public int Priority { get; }

    public void TakeAction();

    public void Release();
  }

  public class ActionTakerImpl : ActionTaker {
    public event Action OnActionsReady;
    public event Action OnActionsDone;
    public int ActionCountAvailable { get; private set; }
    public int ActionCountAllocated { get;}
    public int Priority { get; }

    private readonly Action<ActionTakerImpl> actionTaken;
    private readonly Action<ActionTakerImpl> released;
    public ActionTakerImpl(int actions, int priority, Action<ActionTakerImpl> actionTaken, Action<ActionTakerImpl> released) {
      Priority = priority;
      ActionCountAvailable = actions;
      ActionCountAllocated = actions;
      this.actionTaken = actionTaken;
      this.released = released;
    }

    public void TakeAction() {
      ActionCountAvailable--;
      actionTaken?.Invoke(this);
    }

    public void NotifyReady() {
      ActionCountAvailable = ActionCountAllocated;
      OnActionsReady?.Invoke();
    }

    public void NotifyDone() {
      ActionCountAvailable = 0;
      OnActionsDone?.Invoke();
    }

    public void Release() {
      released?.Invoke(this);
      OnActionsDone = null;
      OnActionsReady = null;
    }
  }
}
