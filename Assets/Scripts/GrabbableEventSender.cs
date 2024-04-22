using UnityEngine;
using Oculus.Interaction;
using UltEvents;

public class ObjectGrabbedEventSender : OneGrabFreeTransformer, ITransformer
{
    public UltEvent OnGrabbed = new UltEvent();
    public UltEvent OnReleased = new UltEvent();

    public new void Initialize(IGrabbable grabbable)
    {
        base.Initialize(grabbable);
    }
    public new void BeginTransform()
    {
        base.BeginTransform();
        OnGrabbed?.Invoke();
    }

    //public new void UpdateTransform()
    //{
    //    base.UpdateTransform();
    //    onObjectMoved?.Invoke(gameObject);
    //}

    public new void EndTransform()
    {
        //Parent class does nothing with that method so no need to call it
        OnReleased?.Invoke();
    }
}