using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._WF.JoinReminder;

[Prototype("joinReminder")]
public sealed class JoinReminderPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public LocId Message;

    [DataField(required: true)]
    public List<ProtoId<JobPrototype>> Jobs = new();
}
