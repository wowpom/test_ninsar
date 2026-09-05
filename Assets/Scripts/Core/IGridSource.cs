using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Core
{
    public interface IGridSource
    {
        UniTask<OperationResult<ISymbolGrid>> LoadAsync(CancellationToken cancellationToken);
    }
}
