using Content.Server.Chat.Managers;
using Content.Shared.GameTicking;
using Content.Shared._WF.JoinReminder;
using Robust.Shared.Prototypes;

namespace Content.Server._WF.JoinReminder;

public sealed class JoinReminderSystem : EntitySystem
{
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);
    }

    private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        if (args.Silent || args.JobId == null)
            return;

        foreach (var reminder in _proto.EnumeratePrototypes<JoinReminderPrototype>())
        {
            if (reminder.Jobs.Contains(args.JobId))
                _chat.DispatchServerMessage(args.Player, Loc.GetString(reminder.Message));
        }
    }
}
