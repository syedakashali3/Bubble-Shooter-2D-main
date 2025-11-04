using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarsFillParent : MonoBehaviour
{
    public List<GameObject> childObjects;

    void Start()
    {
        // ✅ Get only direct children (ignore the parent)
        childObjects = GetComponentsInChildren<RectTransform>()
            .Where(rt => rt != transform) // ignore parent
            .Select(rt => rt.gameObject)
            .ToList();

        RandomlyActivateChildren();
    }

    void RandomlyActivateChildren()
    {
        // ✅ First, disable all
        foreach (var obj in childObjects)
            obj.SetActive(false);

        // ✅ Randomly decide how many to activate (1 to all)
        int countToActivate = Random.Range(1, childObjects.Count + 1);

        // ✅ Randomly pick that many unique children
        var selected = childObjects.OrderBy(x => Random.value).Take(countToActivate);

        foreach (var obj in selected)
            obj.SetActive(true);
    }
}
