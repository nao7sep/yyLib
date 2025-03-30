// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Disables the warning regarding local variable type usage (e.g., var vs. explicit type).
// This allows the project to define its own convention for local variable declarations.
[assembly: SuppressMessage ("Style", "IDE0008")]

// Disables the warning requiring braces for single-line statements.
// This allows the existing code style (with or without braces) to remain unchanged.
[assembly: SuppressMessage ("Style", "IDE0011")]

// Disables the warning for simplifying deconstruction.
// This allows explicit deconstruction syntax to remain as-is.
[assembly: SuppressMessage ("Style", "IDE0042")]

// Disables the warning for converting to a conditional expression.
// This allows 'if-else' statements to remain without being converted to conditional expressions.
[assembly: SuppressMessage ("Style", "IDE0045")]

// Disables the warning related to code formatting and indentation.
// This allows the existing formatting preferences to remain in place.
[assembly: SuppressMessage ("Style", "IDE0055")]

// Disables the warning for ignoring the return value of method calls.
// This allows method calls used solely for side effects to remain explicit.
[assembly: SuppressMessage ("Style", "IDE0058")]

// Disables the warning related to explicit property accessor bodies (IDE0061).
// This allows the use of traditional getter/setter implementations instead of expression-bodied members.
[assembly: SuppressMessage ("Style", "IDE0061")]

// Disables the warning for removing unnecessary usage of 'this' or other member qualifiers.
// This allows retaining explicit qualifiers to maintain clarity or consistency.
[assembly: SuppressMessage ("Style", "IDE0100")]

// Disables the warning for mismatched namespace and file path.
// This allows namespaces to differ from the file path structure.
[assembly: SuppressMessage ("Style", "IDE0130")]

// Disables the warning concerning newer C# feature suggestions (e.g., expression-bodied members).
// This allows the codebase to maintain a more traditional or explicit style.
[assembly: SuppressMessage ("Style", "IDE0200")]
