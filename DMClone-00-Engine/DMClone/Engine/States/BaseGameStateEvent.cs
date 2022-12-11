namespace DMClone.Engine.States
{
    internal class BaseGameStateEvent
    {
        internal class Nothing : BaseGameStateEvent { }
        internal class GameQuit : BaseGameStateEvent { }
        internal class GameTick : BaseGameStateEvent { }
        internal class SoundStop: BaseGameStateEvent { }
    }
}
