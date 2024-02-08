using UnityEngine;
using ReactUnity.Reactive;

namespace Kukumberman.ReactUnity.Extra
{
    public sealed class UnityLifecycleBehaviour : MonoBehaviour
    {
        public ReactiveValue<int> OnUpdateEvent = new(0);
        public ReactiveValue<int> OnFixedUpdateEvent = new(0);

        private void Update()
        {
            OnUpdateEvent.Value += 1;
        }

        private void FixedUpdate()
        {
            OnFixedUpdateEvent.Value += 1;
        }
    }
}
