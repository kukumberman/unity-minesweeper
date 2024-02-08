using System.Linq;
using UnityEngine.UIElements;
using ReactUnity.UIToolkit;

namespace Kukumberman.ReactUnity.UIToolkit
{
    public sealed class RadioButtonGroupComponent : ValueComponent<RadioButtonGroup, int>
    {
        public static readonly string ComponentTag = "radioButtonGroup";

        public RadioButtonGroupComponent(UIToolkitContext context, string tag)
            : base(context, tag) { }

        public override void SetProperty(string property, object value)
        {
            if (property == "choices")
            {
                var array = Context.Script.Engine
                    .TraverseScriptArray(value)
                    .Select(entry => entry.ToString())
                    .ToArray();

                Element.choices = array;
            }
            else if (property == "value" && value is ChangeEvent<int> evt)
            {
                base.SetProperty(property, evt.newValue);
            }
            else
            {
                base.SetProperty(property, value);
            }
        }
    }
}
