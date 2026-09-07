using System;
using Game.Core;
using Game.Domain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Presentation
{
    public sealed class InputSystemMoveInput : IMoveInput
    {
        private const float DeadZoneSqr = 0.25f;

        private readonly GameControls _controls;
        private readonly Action<InputAction.CallbackContext> _handler;

        private bool _disposed;

        public InputSystemMoveInput(GameControls controls)
        {
            _controls = controls;
            _handler = OnMove;
            _controls.Gameplay.Move.performed += _handler;
        }

        public event Action<MoveDirection> Moved;

        public void Enable()
        {
            if (_disposed)
            {
                return;
            }

            _controls.Gameplay.Move.Enable();
        }

        public void Disable()
        {
            if (_disposed)
            {
                return;
            }

            _controls.Gameplay.Move.Disable();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _controls.Gameplay.Move.performed -= _handler;
            _controls.Gameplay.Move.Disable();
            Moved = null;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!TryReadDirection(context.ReadValue<Vector2>(), out var direction))
            {
                return;
            }

            Moved?.Invoke(direction);
        }

        private static bool TryReadDirection(Vector2 value, out MoveDirection direction)
        {
            direction = default;

            if (value.sqrMagnitude < DeadZoneSqr)
            {
                return false;
            }

            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
            {
                direction = value.x > 0f ? MoveDirection.Right : MoveDirection.Left;
                return true;
            }

            direction = value.y > 0f ? MoveDirection.Up : MoveDirection.Down;
            return true;
        }
    }
}
