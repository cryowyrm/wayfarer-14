using Content.Client.DisplacementMap;
using Content.Shared._WF.Clothing;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Content.Client.Inventory;
using Content.Shared._NF.DisplacementMap.Components;
using Content.Shared.Humanoid;
using static Robust.Client.GameObjects.SpriteComponent;

namespace Content.Client._WF.Clothing;

public sealed class BadgeHolderVisualsSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly DisplacementMapSystem _displacement = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private const string NeckSlot = "neck";
    private const string NeckState = "equipped-NECK";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BadgeHolderComponent, EquipmentVisualsUpdatedEvent>(OnEquipmentVisualsUpdated);
    }

    private void OnEquipmentVisualsUpdated(EntityUid uid, BadgeHolderComponent component, EquipmentVisualsUpdatedEvent args)
    {
        if (!_container.TryGetContainer(uid, BadgeHolderComponent.ContainerId, out var container)
            || container is not ContainerSlot { ContainedEntity: { } badgeUid }
            || !TryComp<ClothingComponent>(badgeUid, out var badge)
            || !TryComp<SpriteComponent>(args.Equipee, out var sprite)
            || !TryComp<InventoryComponent>(args.Equipee, out var inventory)
            || !TryComp<InventorySlotsComponent>(args.Equipee, out var inventorySlots)
            || !_inventory.TryGetSlot(args.Equipee, NeckSlot, out var neckSlot, inventory)
            || !_sprite.LayerMapTryGet((args.Equipee, sprite), NeckSlot, out var index, false))
            return;

        index += 1 + (inventorySlots.VisualLayerKeys.GetValueOrDefault(NeckSlot)?.Count ?? 0);

        var key = $"pinned-badge-{args.Slot}";
        index = _sprite.AddLayer((args.Equipee, sprite), new PrototypeLayerData
        {
            RsiPath = badge.RsiPath,
            State = NeckState,
            Offset = neckSlot.Offset,
        }, index);
        _sprite.LayerMapSet((args.Equipee, sprite), key, index);
        args.RevealedLayers.Add(key);

        if (sprite[index] is Layer { RSI: { } rsi } layer
            && inventory.SpeciesId != null
            && rsi.TryGetState($"{NeckState}-{inventory.SpeciesId}", out _))
        {
            _sprite.LayerSetRsiState(layer, $"{NeckState}-{inventory.SpeciesId}");
            return;
        }

        if (HasComp<DisableDisplacementsComponent>(args.Equipee))
            return;

        var displacements = CompOrNull<HumanoidAppearanceComponent>(args.Equipee)?.Sex switch
        {
            Sex.Male when inventory.MaleDisplacements.Count > 0 => inventory.MaleDisplacements,
            Sex.Female when inventory.FemaleDisplacements.Count > 0 => inventory.FemaleDisplacements,
            _ => inventory.Displacements,
        };

        if (displacements.GetValueOrDefault(NeckSlot) is { } displacement
            && _displacement.TryAddDisplacement(displacement, (args.Equipee, sprite), index, key, out var displacementKey))
            args.RevealedLayers.Add(displacementKey);
    }
}
