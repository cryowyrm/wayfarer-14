using Content.Shared.Clothing.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Verbs;
using Robust.Shared.Containers;

namespace Content.Shared._WF.Clothing;

public sealed class PinnableBadgeSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedItemSystem _item = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PinnableBadgeComponent, GetVerbsEvent<UtilityVerb>>(OnGetAttachVerb);
        SubscribeLocalEvent<PinnableBadgeComponent, EntGotInsertedIntoContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<PinnableBadgeComponent, EntGotRemovedFromContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<BadgeHolderComponent, GetVerbsEvent<AlternativeVerb>>(OnGetRemoveVerb);
    }

    private void OnGetAttachVerb(Entity<PinnableBadgeComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var target = args.Target;
        if (!TryComp<ClothingComponent>(target, out var clothing) || (clothing.Slots & ent.Comp.Slots) == 0)
            return;

        if (_container.TryGetContainer(target, BadgeHolderComponent.ContainerId, out var existing)
            && existing.ContainedEntities.Count > 0)
            return;

        var badge = ent.Owner;
        args.Verbs.Add(new UtilityVerb
        {
            Text = Loc.GetString("pinnable-badge-verb-attach"),
            Act = () =>
            {
                EnsureComp<BadgeHolderComponent>(target);
                var container = _container.EnsureContainer<ContainerSlot>(target, BadgeHolderComponent.ContainerId);
                _container.Insert(badge, container);
            },
        });
    }

    private void OnContainerModified(EntityUid uid, PinnableBadgeComponent component, ContainerModifiedMessage args)
    {
        if (args.Container.ID == BadgeHolderComponent.ContainerId)
            _item.VisualsChanged(args.Container.Owner);
    }

    private void OnGetRemoveVerb(Entity<BadgeHolderComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.Hands == null)
            return;

        if (!_container.TryGetContainer(ent, BadgeHolderComponent.ContainerId, out var container)
            || container is not ContainerSlot { ContainedEntity: { } badge })
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("pinnable-badge-verb-remove"),
            Priority = -1,
            Act = () =>
            {
                _container.Remove(badge, container);
                _hands.PickupOrDrop(user, badge);
            },
        });
    }
}
