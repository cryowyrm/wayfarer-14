namespace Content.Server._WF.Forensics;

[RegisterComponent, Access(typeof(ScanLocationSystem))]
public sealed partial class ScanLocationComponent : Component
{
    [DataField]
    public string? Text;
}
