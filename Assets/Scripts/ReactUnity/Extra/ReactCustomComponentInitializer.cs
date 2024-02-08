using UnityEngine;
using ReactUnity.UIToolkit;
using Kukumberman.ReactUnity.UIToolkit;

namespace Kukumberman.ReactUnity.Extra
{
    public sealed class ReactCustomComponentInitializer : MonoBehaviour
    {
        [ReactBeforeStart]
        private void Setup()
        {
            UIToolkitContext.ComponentCreators[RadioButtonGroupComponent.ComponentTag] = (
                type,
                text,
                context
            ) => new RadioButtonGroupComponent(context, type);
        }
    }
}
