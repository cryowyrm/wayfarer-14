using Content.Shared.Inventory;

namespace Content.Shared._WF.Clothing;

[RegisterComponent]
public sealed partial class PinnableBadgeComponent : Component
{
    [DataField]
    public SlotFlags Slots = SlotFlags.INNERCLOTHING | SlotFlags.OUTERCLOTHING;
}
