using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableListener
{
    void OnSelect();
    void OnKeepSelecting();
    void OnDeselect();
}
