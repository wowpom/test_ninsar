using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface IErrorPresenter
    {
        UniTask ShowAsync(string message, CancellationToken cancellationToken);
    }
}
