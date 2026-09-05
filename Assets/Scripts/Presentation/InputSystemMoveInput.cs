using System;
using Game.Core;
using Game.Domain;
using UnityEngine.InputSystem;

namespace Game.Presentation
{
    public sealed class InputSystemMoveInput : IMoveInput
    {
        private const string UpPrimaryBinding = "<Keyboard>/w";
        private const string UpSecondaryBinding = "<Keyboard>/upArrow";
        private const string DownPrimaryBinding = "<Keyboard>/s";
        private const string DownSecondaryBinding = "<Keyboard>/downArrow";
        private const string LeftPrimaryBinding = "<Keyboard>/a";
        private const string LeftSecondaryBinding = "<Keyboard>/leftArrow";
        private const string RightPrimaryBinding = "<Keyboard>/d";
        private const string RightSecondaryBinding = "<Keyboard>/rightArrow";

        private readonly DirectionAction[] _actions;

        private bool _disposed;

        public InputSystemMoveInput()
        {
            _actions = new[]
            {
                CreateAction(MoveDirection.Up, UpPrimaryBinding, UpSecondaryBinding),
                CreateAction(MoveDirection.Down, DownPrimaryBinding, DownSecondaryBinding),
                CreateAction(MoveDirection.Left, LeftPrimaryBinding, LeftSecondaryBinding),
                CreateAction(MoveDirection.Right, RightPrimaryBinding, RightSecondaryBinding),
            };

            foreach (var directionAction in _actions)
            {
                directionAction.Subscribe();
            }
        }

        public event Action<MoveDirection> Moved;

        public void Enable()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var directionAction in _actions)
            {
                directionAction.Enable();
            }
        }

        public void Disable()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var directionAction in _actions)
            {
                directionAction.Disable();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var directionAction in _actions)
            {
                directionAction.Dispose();
            }

            Moved = null;
        }

        private DirectionAction CreateAction(MoveDirection direction, string primaryBinding, string secondaryBinding)
        {
            var action = new InputAction(direction.ToString(), InputActionType.Button);

            action.AddBinding(primaryBinding);
            action.AddBinding(secondaryBinding);

            Action<InputAction.CallbackContext> handler = context =>
            {
                Moved?.Invoke(direction);
            };

            return new DirectionAction(action, handler);
        }

        private sealed class DirectionAction
        {
            private readonly InputAction _action;
            private readonly Action<InputAction.CallbackContext> _handler;

            public DirectionAction(InputAction action, Action<InputAction.CallbackContext> handler)
            {
                _action = action;
                _handler = handler;
            }

            public void Subscribe()
            {
                _action.performed += _handler;
            }

            public void Enable()
            {
                _action.Enable();
            }

            public void Disable()
            {
                _action.Disable();
            }

            public void Dispose()
            {
                _action.performed -= _handler;
                _action.Disable();
                _action.Dispose();
            }
        }
    }
}
