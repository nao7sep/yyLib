using System.Text.RegularExpressions;

namespace yyLib
{
    public static partial class yySql
    {
        private static readonly Lazy <string []> _reservedKeywords = new (() =>
        [
            // The following keywords appear multiple times in the list:
            //     ANALYZE, CLUSTER, DEFERRED, EXCLUSIVE, IMMEDIATE, LOAD, MATCH, RULE, SAVEPOINT, WITHOUT
            // Also, there should be missing/incorrect keywords.
            // I think that is OK because the primary objective of this list is to avoid confusion and improve code readability.
            // If a keyword that isnt present in the list is used as an object name, there wont be any significant risks.

            // Frequently used
            "SELECT", "INSERT", "UPDATE", "DELETE", "FROM", "WHERE", "JOIN", "TABLE",

            // Commonly used
            "ADD", "ALL", "ALTER", "AND", "AS", "BETWEEN", "BY", "CASE", "CHECK", "COLUMN",
            "CONSTRAINT", "CREATE", "DEFAULT", "DISTINCT", "DROP", "EXEC", "EXISTS", "FOREIGN",
            "GROUP", "HAVING", "IN", "INDEX", "INNER", "IS", "KEY", "LEFT", "LIKE", "LIMIT",
            "NOT", "NULL", "ON", "OR", "ORDER", "OUTER", "PRIMARY", "REFERENCES", "RIGHT",
            "SCHEMA", "SET", "TOP", "TRUNCATE", "UNION", "UNIQUE", "USE", "VALUES", "VIEW", "WITH",

            // SQLite-specific
            "ABORT", "AFTER", "ANALYZE", "ATTACH", "AUTOINCREMENT", "BEFORE", "CASCADE", "CLUSTER",
            "DEFERRED", "DETACH", "EXCLUSIVE", "EXPLAIN", "FAIL", "IGNORE", "IMMEDIATE", "INITIALLY",
            "INSTEAD", "MATCH", "PLAN", "PRAGMA", "REINDEX", "RELEASE", "RENAME", "ROLLBACK",
            "SAVEPOINT", "TEMP", "TRIGGER", "VACUUM", "WITHOUT",

            // SQL Server-specific
            "ACTIVATE", "ANY", "BACKUP", "BREAK", "BROWSE", "CATCH", "CHECKPOINT", "CLUSTERED",
            "COMMIT", "CONTAINS", "CONTAINSTABLE", "CONTINUE", "CURSOR", "DEALLOCATE", "DENY",
            "DUMP", "END", "ERRLVL", "FETCH", "FREETEXT", "FREETEXTTABLE", "GO", "GOTO", "IDENTITY",
            "MERGE", "OPEN", "PIVOT", "PRINT", "RAISERROR", "READ", "REVERT", "ROWCOUNT", "RULE",
            "SAVE", "SESSION_USER", "TRY", "UNPIVOT", "WAITFOR", "WHILE", "WRITETEXT",

            // MySQL-specific
            "ACCESSIBLE", "ANALYZE", "ARCHIVE", "CROSS", "CUME_DIST", "DATABASE", "DELIMITER",
            "DESCRIBE", "DO", "DUAL", "DUMPFILE", "ENUM", "ESCAPE", "EXIT", "FORCE", "FULLTEXT",
            "GEOMETRY", "HIGH_PRIORITY", "INTERVAL", "KILL", "LINES", "LOAD", "LOCALTIMESTAMP",
            "LOW_PRIORITY", "MASTER", "MATCH", "MEDIUMTEXT", "NATURAL", "OPTIMIZE", "PROCEDURE",
            "RANK", "REGEXP", "REPLACE", "REQUIRE", "SHOW", "SPATIAL", "SQLSTATE", "STRAIGHT_JOIN",
            "TERMINATED", "UNDO", "UNLOCK", "USAGE", "UTC_TIMESTAMP",

            // PostgreSQL-specific
            "ABSOLUTE", "ALLOCATE", "ANALYSE", "ASSERTION", "BACKWARD", "BINARY", "CACHE", "CYCLE",
            "DEFERRED", "EACH", "EXCEPTION", "FORWARD", "FUNCTION", "INHERITS", "LANCOMPILER",
            "LARGE", "LISTEN", "LOAD", "MOVE", "OTHERS", "OVER", "OWNER", "PARTIAL", "PREPARE",
            "REASSIGN", "RESET", "REVOKE", "RULE", "SAVEPOINT", "SCROLL", "SEQUENCE", "STATEMENT",
            "STDIN", "STDOUT", "TABLESPACE", "TEMPORARY", "TREAT", "TRUSTED", "UNLISTEN", "WITHOUT",

            // Oracle-specific
            "ACCESS", "AUDIT", "CLUSTER", "COMPRESS", "EXCLUSIVE", "FILE", "FREELIST", "IMMEDIATE",
            "INCREMENT", "INSTANCE", "MAXEXTENTS", "MINUS", "MLSLABEL", "NEXT", "NOAUDIT",
            "NOCOMPRESS", "NOWAIT", "ONLINE", "PCTFREE", "RAW", "RECOVER", "SNAPSHOT", "START",
            "SYSDATE", "UID", "VALIDATE"
        ]);

        public static string [] ReservedKeywords => _reservedKeywords.Value;

        private static readonly Lazy <HashSet <string>> _reservedKeywordsSet = new (() => ReservedKeywords.ToHashSet ());

        public static HashSet <string> ReservedKeywordsSet => _reservedKeywordsSet.Value;

        [GeneratedRegex (@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
        public static partial Regex ObjectNameRegex ();

        public static void ValidateObjectName (string? objectName, bool avoidReservedKeywords = true)
        {
            if (string.IsNullOrEmpty (objectName))
                throw new yyArgumentException ("Object name cannot be null or empty.");

            // Length is currently not checked because the database engine would throw an exception anyway.
            // Also because the max length varies between database engines.
            // This method is meant to block injection attacks and prevent reserved keywords from being used.

            if (ObjectNameRegex ().IsMatch (objectName) == false)
                throw new yyArgumentException ($"Object name '{objectName}' is invalid.");

            if (avoidReservedKeywords && ReservedKeywords.Contains (objectName, StringComparer.OrdinalIgnoreCase))
                throw new yyArgumentException ($"Object name '{objectName}' is a reserved keyword.");
        }
    }
}
