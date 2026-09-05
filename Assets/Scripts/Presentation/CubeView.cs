using UnityEngine;

namespace Game.Presentation
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class CubeView : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private MeshRenderer _renderer;

        public void Apply(Material material)
        {
            if (ReferenceEquals(_renderer.sharedMaterial, material))
            {
                return;
            }

            _renderer.sharedMaterial = material;
        }

        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<MeshRenderer>();
            }
        }
    }
}
