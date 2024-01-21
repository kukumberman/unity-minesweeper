using Core;
using UnityEngine.UIElements;

public abstract class VisualElementWithModel<T> : VisualElement, IBehaviourWithModel<T>, IObserver
    where T : Observable
{
    private T _model;

    #region IBehaviourWithModel
    public T Model
    {
        get { return _model; }
        set
        {
            if (null != _model)
            {
                _model.RemoveObserver(this);
            }

            _model = value;
            OnApplyModel(value);

            if (null != _model)
            {
                _model.AddObserver(this);
                OnModelChanged(_model);
            }
        }
    }
    #endregion

    #region IObserver
    public void OnObjectChanged(Observable observable)
    {
        if (observable is T model)
        {
            OnModelChanged(model);
        }
        else
        {
            OnModelChanged(Model);
        }
    }
    #endregion

    protected abstract void OnModelChanged(T model);

    protected virtual void OnApplyModel(T model) { }
}
