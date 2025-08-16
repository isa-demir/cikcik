using UnityEngine;

public class StateController : MonoBehaviour
{
    private PlayerState _currentPlayerState = PlayerState.Idle;

    public PlayerState GetCurrentState => _currentPlayerState;

    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }
    public void ChangeState(PlayerState newState)
    {
        if (newState == _currentPlayerState)
        {
            return;
        }
        _currentPlayerState = newState;
    }
}
