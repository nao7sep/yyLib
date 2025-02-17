﻿// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Suppresses the warning about not providing the required constructors for a custom exception class (CA1032).
// This is usually a minor design issue and comes up only in analysis mode set to All.
[assembly: SuppressMessage ("Design", "CA1032")]

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

// Disables the warning for converting 'if' statements to expressions.
// This allows traditional 'if' statements to remain instead of being rewritten as conditional expressions.
[assembly: SuppressMessage ("Style", "IDE0046")]

// Disables the warning for using simplified method invocations.
// This allows the use of more explicit method calls instead of suggested shorthand or simplified syntax.
[assembly: SuppressMessage ("Style", "IDE0057")]

// Disables the warning for mismatched namespace and file path.
// This allows namespaces to differ from the file path structure.
[assembly: SuppressMessage ("Style", "IDE0130")]

// Disables the warning for using coalescing expressions.
// This allows traditional null checks and assignments instead of being rewritten as null-coalescing expressions (e.g., '??=').
[assembly: SuppressMessage ("Style", "IDE0270")]

// Disables the warning for naming rule violations.
// This allows non-standard naming conventions, such as those used for legacy code or specific design requirements.
[assembly: SuppressMessage ("Style", "IDE1006")]
