using UnityEngine;
using UnityEngine.UI;

namespace Game.Presentation
{
    public sealed class ErrorOverlayView : MonoBehaviour
    {
        [SerializeField] private Text _message;

        public void Apply(string message)
        {
            if (_message == null)
            {
                throw new MissingReferenceException(
                    $"{nameof(ErrorOverlayView)} на «{name}»: не назначен {nameof(Text)}.");
            }

            _message.text = message;
        }
    }
}
