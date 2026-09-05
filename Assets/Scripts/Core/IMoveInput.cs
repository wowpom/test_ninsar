using System;
using Game.Domain;

namespace Game.Core
{
    public interface IMoveInput : IDisposable
    {
        event Action<MoveDirection> Moved;

        void Enable();

        void Disable();
    }
}
