// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Disables the warning for converting 'if' statements to expressions.
// This allows traditional 'if' statements to remain instead of being rewritten as conditional expressions.
[assembly: SuppressMessage("Style", "IDE0046")]

// Disables the warning for using simplified method invocations.
// This allows the use of more explicit method calls instead of suggested shorthand or simplified syntax.
[assembly: SuppressMessage("Style", "IDE0057")]

// Disables the warning for mismatched namespace and file path.
// This allows namespaces to differ from the file path structure.
[assembly: SuppressMessage("Style", "IDE0130")]

// Disables the warning for using coalescing expressions.
// This allows traditional null checks and assignments instead of being rewritten as null-coalescing expressions (e.g., '??=').
[assembly: SuppressMessage("Style", "IDE0270")]

// Disables the warning for naming rule violations.
// This allows non-standard naming conventions, such as those used for legacy code or specific design requirements.
[assembly: SuppressMessage("Style", "IDE1006")]
