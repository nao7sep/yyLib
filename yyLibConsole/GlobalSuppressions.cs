// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Disables the warning for simplifying deconstruction.
// This allows explicit deconstruction syntax to remain as-is.
[assembly: SuppressMessage("Style", "IDE0042")]

// Disables the warning for converting to a conditional expression.
// This allows 'if-else' statements to remain without being converted to conditional expressions.
[assembly: SuppressMessage("Style", "IDE0045")]

// Disables the warning for mismatched namespace and file path.
// This allows namespaces to differ from the file path structure.
[assembly: SuppressMessage("Style", "IDE0130")]
