using System;

public sealed class GameTimer 
{
    readonly float _interval;
    readonly bool  _loop;

    float _timer;

    bool _isActive;

    Action _onFinish;

    public GameTimer(float interval, Action onFinish, bool loop = false) 
    {
        _interval = interval;
        _timer    = 0f;

        _onFinish = onFinish;
        _loop     = loop;
        
        UnityContext.AddUpdateAction(Update);
    }
    
    void Stop()
    {
        _onFinish = null;
        UnityContext.RemoveUpdateAction(Update);
    }

    void Update() 
    {
        _timer += PauseController.CurDeltaTime;
        if ( _timer >= _interval ) 
        {
            _onFinish?.Invoke();
            if ( !_loop ) 
            {
                Stop();
            }
        }
    }
}
