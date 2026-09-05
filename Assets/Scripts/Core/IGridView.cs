using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Core
{
    public interface IGridView
    {
        UniTask BuildAsync(int windowSize, CancellationToken cancellationToken);

        void Render(GridWindow window);
    }
}
