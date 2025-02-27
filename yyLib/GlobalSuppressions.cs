// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Suppresses the warning about not providing the required constructors for a custom exception class (CA1032).
// This is usually a minor design issue and comes up only in analysis mode set to All.
[assembly: SuppressMessage ("Design", "CA1032")]

// Suppresses the warning about passing strings in URI parameters (CA1054).
// This allows the codebase to accept or manipulate string URIs for flexibility.
[assembly: SuppressMessage("Design", "CA1054")]

// Suppresses the warning for using strings in URIs (CA1056).
// This might be intentional for flexibility, rather than strictly using System.Uri.
[assembly: SuppressMessage("Design", "CA1056")]

// Suppresses the warning about validating arguments for public methods (CA1062).
// This is a minor design concern flagged in analysis mode set to All, and may not always be applicable depending on the use case.
[assembly: SuppressMessage ("Design", "CA1062")]

// Disables the warning for not specifying an IFormatProvider in formatting methods (CA1305).
// This warning appears only when the analysis mode is set to "Recommended," which is stricter than the default "Default" mode.
// I am fully aware of potential culture-related errors. To prevent these issues, I ensure that all values are properly converted to strings
// before formatting them, thereby avoiding culture-specific ambiguities.
[assembly: SuppressMessage ("Globalization", "CA1305")]

// Disables the warning for naming conventions of generic type parameters (CA1715).
// This warning appears only when the analysis mode is set to "Recommended," which is stricter than the default "Default" mode.
// It allows generic type parameters to use non-standard naming conventions instead of the recommended 'T' prefix.
[assembly: SuppressMessage ("Naming", "CA1715")]

// Suppresses the warning about exposing arrays as public properties (CA1819).
// This performance-related warning is of low importance and appears only in analysis mode set to All.
[assembly: SuppressMessage ("Performance", "CA1819")]

// Suppresses the warning about exposing writable properties for collections (CA2227).
// Collection properties are made writable intentionally to distinguish between "unset" (null) and "set as empty" ([])
// when serialized to JSON. This distinction is important for proper data representation and serialization.
[assembly: SuppressMessage ("Usage", "CA2227")]

// Suppresses the warning for calling methods in an unsafe way (CA2234).
// This enables calling certain methods flagged by analysis without refactoring each call site.
[assembly: SuppressMessage("Usage", "CA2234")]

// Disables the warning regarding local variable type usage (IDE0008).
// This allows the project to define its own convention for using 'var' vs. explicit types.
[assembly: SuppressMessage("Style", "IDE0008")]

// Disables the warning requiring braces for single-line statements (IDE0011).
// This allows either style—braced or unbraced—for single-line conditionals and loops.
[assembly: SuppressMessage("Style", "IDE0011")]

// Disables the warning about unnecessary parentheses or simpler code constructs (IDE0022).
// This preserves explicit parentheses or statements for clarity if desired.
[assembly: SuppressMessage("Style", "IDE0022")]

// Disables the warning about using auto-properties where possible (IDE0032).
// This allows continued use of explicit backing fields where beneficial or clearer.
[assembly: SuppressMessage("Style", "IDE0032")]

// Disables the warning for simplifying deconstruction (IDE0042).
// This allows explicit deconstruction syntax rather than the compiler-suggested simplification.
[assembly: SuppressMessage("Style", "IDE0042")]

// Disables the warning for converting 'if' statements to expressions.
// This allows traditional 'if' statements to remain instead of being rewritten as conditional expressions.
[assembly: SuppressMessage ("Style", "IDE0046")]

// Disables the warning about code formatting and indentation (IDE0055).
// This allows the existing formatting preferences to remain intact.
[assembly: SuppressMessage("Style", "IDE0055")]

// Disables the warning for using simplified method invocations.
// This allows the use of more explicit method calls instead of suggested shorthand or simplified syntax.
[assembly: SuppressMessage ("Style", "IDE0057")]

// Disables the warning for ignoring return values (IDE0058).
// This allows method calls used solely for side effects without assigning or using their results.
[assembly: SuppressMessage("Style", "IDE0058")]

// Disables the warning for removing unneeded 'this' qualifiers (IDE0100).
// This preserves explicit references to current instance members when desired.
[assembly: SuppressMessage("Style", "IDE0100")]

// Disables the warning for mismatched namespace and file path.
// This allows namespaces to differ from the file path structure.
[assembly: SuppressMessage ("Style", "IDE0130")]

// Disables the warning for suggesting newer C# constructs (IDE0200).
// This allows a more traditional or explicit style to be maintained where preferred.
[assembly: SuppressMessage("Style", "IDE0200")]

// Disables the warning for using coalescing expressions.
// This allows traditional null checks and assignments instead of being rewritten as null-coalescing expressions (e.g., '??=').
[assembly: SuppressMessage ("Style", "IDE0270")]

// Disables the warning for code style preferences like spacing or newlines (IDE0305).
// This allows adherence to project-specific formatting guidelines.
[assembly: SuppressMessage("Style", "IDE0305")]

// Disables the warning for naming rule violations.
// This allows non-standard naming conventions, such as those used for legacy code or specific design requirements.
[assembly: SuppressMessage ("Style", "IDE1006")]
