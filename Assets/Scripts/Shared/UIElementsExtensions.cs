using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsExtensions
{
    public static void SetTopLeft(VisualElement element, Vector2 position)
    {
        element.style.left = new StyleLength(new Length(position.x, LengthUnit.Pixel));
        element.style.top = new StyleLength(new Length(position.y, LengthUnit.Pixel));
    }

    public static Vector2 GetTopLeft(VisualElement element)
    {
        var position = Vector2.zero;
        position.x = element.resolvedStyle.left;
        position.y = element.resolvedStyle.top;
        return position;
    }
}
