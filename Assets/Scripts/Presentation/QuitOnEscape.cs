using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Presentation
{
    public sealed class QuitOnEscape : IStartable, IDisposable
    {
        private readonly GameControls _controls;
        private readonly Action<InputAction.CallbackContext> _handler;

        private bool _disposed;

        public QuitOnEscape(GameControls controls)
        {
            _controls = controls;
            _handler = OnPerformed;
            _controls.Gameplay.Quit.performed += _handler;
        }

        public void Start()
        {
            if (_disposed)
            {
                return;
            }

            _controls.Gameplay.Quit.Enable();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _controls.Gameplay.Quit.performed -= _handler;
            _controls.Gameplay.Disable();
        }

        private static void OnPerformed(InputAction.CallbackContext context)
        {
            Application.Quit();
        }
    }
}
