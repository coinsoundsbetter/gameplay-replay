using FishNet.Connection;
using FishNet.Observing;
using UnityEngine;

[CreateAssetMenu(menuName = "FishNet/Observers/Server Only Condition", fileName = "New Host Only Condition")]
public class ServerOnlyCondition : ObserverCondition {
    public override bool ConditionMet(NetworkConnection connection, bool currentlyAdded, out bool notProcessed) {
        notProcessed = false;
        return false;
    }

    public override ObserverConditionType GetConditionType() {
        return ObserverConditionType.Normal;
    }
}
