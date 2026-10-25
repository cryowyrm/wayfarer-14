using Content.Server.GameTicking;

namespace Content.Server._WF.Forensics;

public sealed class ScanLocationSystem : EntitySystem
{
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public void Record(EntityUid scanner, EntityUid target)
    {
        var xform = Transform(target);
        var grid = xform.GridUid is { } gridUid ? Name(gridUid) : Loc.GetString("scan-location-space");
        var tile = _transform.GetGridOrMapTilePosition(target, xform);
        var world = _transform.GetWorldPosition(xform).Floored();
        var duration = _ticker.RoundDuration();
        var time = duration.TotalDays >= 1 ? duration.ToString(@"d\:hh\:mm") : duration.ToString(@"hh\:mm");

        EnsureComp<ScanLocationComponent>(scanner).Text = Loc.GetString("scan-location-text",
            ("time", time),
            ("grid", grid),
            ("x", tile.X),
            ("y", tile.Y),
            ("worldX", world.X),
            ("worldY", world.Y));
    }

    public void Clear(EntityUid scanner)
    {
        RemComp<ScanLocationComponent>(scanner);
    }

    public string GetText(EntityUid scanner)
    {
        return CompOrNull<ScanLocationComponent>(scanner)?.Text is { } text ? text + "\n\n" : string.Empty;
    }
}
