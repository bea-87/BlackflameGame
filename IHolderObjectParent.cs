using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHolderObjectParent {

    public Transform GetHolderObjectFollowTransform();

    public void SetHolderObject(HolderObject holderObject);

    public HolderObject GetHolderObject();

    public void ClearHolderObject();

    public bool HasHolderObject();
}
