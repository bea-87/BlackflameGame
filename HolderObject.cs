using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderObject : ISelectableObject {
    private IHolderObjectParent holderObjectParent;


    public void SetHolderObjectParent(IHolderObjectParent holderObjectParent) {
        this.holderObjectParent = holderObjectParent;


        if (holderObjectParent != null) {
            holderObjectParent.SetHolderObject(this);
            Debug.Log("holderObjectSet");

            transform.parent = holderObjectParent.GetHolderObjectFollowTransform();
            transform.localPosition = Vector3.zero;
        } else {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) {
                transform.position = hit.point + Vector3.up * 0.2f;
            }
            transform.parent = null;
        }
    }

    public IHolderObjectParent GetHolderObjectParent() {
        return holderObjectParent;
    }

    public void DestroySelf() {
        holderObjectParent.ClearHolderObject();

        Destroy(gameObject);
    }

    public override void Interact(Player player) {
        if (holderObjectParent == null) {
            SetHolderObjectParent(player);
        } else {
            holderObjectParent.ClearHolderObject();
            SetHolderObjectParent(null);

        }
    }
}
