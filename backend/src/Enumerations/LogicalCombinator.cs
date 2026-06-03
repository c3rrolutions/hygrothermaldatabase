using System.Diagnostics.CodeAnalysis;

namespace Database.Enumerations;

/// <summary>
/// How are multiple access policies evaluated together?
/// </summary>
[SuppressMessage("Naming", "CA1707")]
public enum LogicalCombinator
{
    /// <summary>
    /// Conjunctive logic: Every single condition must evaluate to true (logical and)
    /// </summary>
    ALL,

    /// <summary>
    ///Disjunctive logic: At least one condition must evaluate to true (logical or)
    /// </summary>
    SOME
}