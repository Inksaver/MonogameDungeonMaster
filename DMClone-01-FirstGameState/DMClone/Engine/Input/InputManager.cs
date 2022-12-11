using Microsoft.Xna.Framework.Input;
using System;

namespace DMClone.Engine.Input
{
    internal class InputManager
    {
        private readonly BaseInputMapper inputMapper;
        public InputManager(BaseInputMapper inputMapper)
        {
            this.inputMapper = inputMapper;
        }
        public void GetCommands(Action<BaseInputCommand> actOnState)
        {
            var keyboardState = Keyboard.GetState();
            foreach (var state in inputMapper.GetKeyboardState(keyboardState))
            {
                actOnState(state);
            }
            var mouseState = Mouse.GetState();
            foreach (var state in inputMapper.GetMouseState(mouseState))
            {
                actOnState(state);
            }
            // assume only 1 gamepad is being used
            var gamePadState = GamePad.GetState(0);
            foreach (var state in inputMapper.GetGamePadState(gamePadState))
            {
                actOnState(state);
            }
        }
    }
}
