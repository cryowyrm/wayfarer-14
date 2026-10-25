using Robust.Shared.GameStates;

namespace Content.Shared._WF.Clothing;

[RegisterComponent, NetworkedComponent]
public sealed partial class BadgeHolderComponent : Component
{
    public const string ContainerId = "pinned_badge";
}
