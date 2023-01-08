namespace PixelByProxy.Asus.Router.Messages;

internal class ErrorStrings
{
    /// <summary>
    /// Write access must be enabled in the AsusConfiguration in order to call this method.
    /// </summary>
    internal const string EnableWriteAccess
        = "Write access must be enabled in the AsusConfiguration in order to call this method.";

    /// <summary>
    /// A valid {propertyName} is required.
    /// </summary>
    internal static string ValidPropertyIsRequired(string propertyName)
        => $"A valid {propertyName} is required.";

    /// <summary>
    /// {propertyName} cannot contain characters '{invalidCharacters}'.
    /// </summary>
    internal static string PropertyCannotContainCharacters(string propertyName, char[] invalidCharacters)
        => $"{propertyName} cannot contain characters '{string.Join(',', invalidCharacters)}'.";

    /// <summary>
    /// The port must be between {minPort} and {maxPort}.
    /// </summary>
    internal static string PortBetween(int minPort, int maxPort)
        => $"The port must be between {minPort} and {maxPort}.";

    /// <summary>
    /// The end port ({endPort}) must greater than the start port ({startPort}).
    /// </summary>
    internal static string EndPortMustBeGreater(int startPort, int endPort)
        => $"The end port ({endPort}) must greater than the start port ({startPort}).";

    /// <summary>
    /// The rule list cannot exceed {maxRules} items.
    /// </summary>
    internal static string RulesCannotExceed(int maxRules)
        => $"The rule list cannot exceed {maxRules} items.";
}