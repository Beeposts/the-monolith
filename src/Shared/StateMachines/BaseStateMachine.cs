using Stateless;
// ReSharper disable VirtualMemberCallInConstructor

namespace Shared.StateMachines;

public abstract class BaseStateMachine<TState, TStateReason>
    where TState : struct, Enum
    where TStateReason : struct, Enum
{
    protected readonly StateMachine<TState, TStateReason> StateMachine;

    protected BaseStateMachine(TState initialState)
    {
        StateMachine = new StateMachine<TState, TStateReason>(initialState);
        Configure();
    }
    protected BaseStateMachine(Func<TState> stateAccessor, Action<TState> stateMutator)
    {
        StateMachine = new StateMachine<TState, TStateReason>(stateAccessor, stateMutator);
        Configure();
    }
    protected abstract void Configure();
    
    public bool CanFire(TStateReason trigger) => StateMachine.CanFire(trigger);

    public TState Fire(TStateReason trigger)
    {
        StateMachine.Fire(trigger);
        return StateMachine.State;
    }
}