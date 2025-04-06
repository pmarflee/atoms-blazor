using Ardalis.SmartEnum;
using System.ComponentModel;
using System.Globalization;

namespace Atoms.Core.Enums;

public enum GameState
{
    Menu = 1,
    Game,
}

public enum MenuState
{
    Menu = 1,
    About
}

public sealed class PlayerTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string)
               || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value is string name && !string.IsNullOrEmpty(name) 
            ? PlayerType.FromName(name) 
            : (object?)null;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return value is PlayerType playerType ? playerType.Name : null;
    }
}

[TypeConverter(typeof(PlayerTypeConverter))]
public sealed class PlayerType : SmartEnum<PlayerType>
{
    public static readonly PlayerType Human = new(nameof(Human), 1, nameof(Human));
    public static readonly PlayerType CPU_Easy = new("CPU (E)", 2, "CPU (Easy)");
    public static readonly PlayerType CPU_Medium = new("CPU (M)", 3, "CPU (Medium)");
    public static readonly PlayerType CPU_Hard = new("CPU (H)", 4, "CPU (Hard)");

    private PlayerType(string name, int value, string description)
        : base(name, value)
    {
        Description = description;
    }

    public string Description { get; }
}

public sealed class ColourSchemeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string)
               || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value is string name && !string.IsNullOrEmpty(name) 
            ? ColourScheme.FromName(name) 
            : (object?)null;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return value is ColourScheme playerType ? playerType.Name : null;
    }
}

[TypeConverter(typeof(ColourSchemeConverter))]
public sealed class ColourScheme(string name, int value)
    : SmartEnum<ColourScheme>(name, value)
{
    public static readonly ColourScheme Original = new(nameof(Original), 1);
    public static readonly ColourScheme Alternate = new(nameof(Alternate), 2);
}

public sealed class AtomShapeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string)
               || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value is string name && !string.IsNullOrEmpty(name) 
            ? AtomShape.FromName(name) 
            : (object?)null;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return value is AtomShape playerType ? playerType.Name : null;
    }
}

[TypeConverter(typeof(AtomShapeConverter))]
public sealed class AtomShape(string name, int value)
    : SmartEnum<AtomShape>(name, value)
{
    public static readonly AtomShape Round = new(nameof(Round), 1);
    public static readonly AtomShape Varied = new(nameof(Varied), 2);
}

public enum ExplosionState
{
    None = 0,
    Before = 1,
    After = 2
}

public enum PlayerMoveResult
{
    None = 0,
    Success = 1,
    InvalidMove = 2,
    GameStateHasChanged = 3
}

public enum InviteResult
{
    None = 0,
    Success = 1,
    GameNotFound = 2,
    PlayerNotFound = 3,
    InviteAlreadyAccepted = 4
}
