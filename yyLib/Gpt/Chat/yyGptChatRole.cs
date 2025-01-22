namespace yyLib
{
    /// <summary>
    /// The integer values associated with these enum items are volatile and may change in future updates.
    /// Avoid relying on the specific values.
    /// </summary>
    public enum yyGptChatRole
    {
        [Obsolete ("Use Developer instead, if the model supports it.")]
        System,

        Developer,
        User,
        Assistant
    }
}
