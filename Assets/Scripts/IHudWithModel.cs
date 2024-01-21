using Core;

namespace Game.UI.Hud
{
    public interface IHudWithModel<T> : IHud
        where T : Observable
    {
        T Model { set; }
    }
}
