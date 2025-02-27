using System.Text.Json.Serialization;

namespace yyLib
{
    public class yyGptChatRequest
    {
        // https://platform.openai.com/docs/api-reference/chat/create
        // https://github.com/openai/openai-dotnet/blob/main/src/Custom/Chat/ChatCompletionOptions.cs

        [JsonPropertyName ("messages")]
        public IList <yyGptChatMessage> Messages { get; } = []; // Required.

        public void AddMessage (string content, yyGptChatRole role, string? name = null)
        {
            Messages.Add (new ()
            {
                Content = content,
                Role = role,
                Name = name
            });
        }

        public void AddDeveloperMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.Developer, name);

        public void AddSystemMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.System, name);

        public void AddUserMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.User, name);

        public void AddUserMessage (IList <yyGptChatContentPart> content, string? name = null)
        {
            Messages.Add (new ()
            {
                Content = content,
                Role = yyGptChatRole.User,
                Name = name
            });
        }

        public void AddAssistantMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.Assistant, name);

        public void AddToolMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.Tool, name);

        public void AddFunctionMessage (string content, string? name = null) =>
            AddMessage (content, yyGptChatRole.Function, name);

        [JsonPropertyName ("model")]
        public required string Model { get; set; }

        [JsonPropertyName ("store")]
        public bool? Store { get; set; }

        [JsonPropertyName ("reasoning_effort")]
        public string? ReasoningEffort { get; set; }

        // Official documentation says: Developer-defined tags and values used for filtering completions in the dashboard.
        // Based on official code including OpenAIModelFactory.cs, it is defined as IDictionary <string, string>.
        // https://github.com/openai/openai-dotnet/blob/main/src/Generated/OpenAIModelFactory.cs

        [JsonPropertyName ("metadata")]
        public IDictionary <string, string>? Metadata { get; set; }

        [JsonPropertyName ("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }

        [JsonPropertyName ("logit_bias")]
        public IDictionary <int, double>? LogitBias { get; set; }

        [JsonPropertyName ("logprobs")]
        public bool? LogProbs { get; set; }

        [JsonPropertyName ("top_logprobs")]
        public int? TopLogProbs { get; set; }

        [JsonPropertyName ("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonPropertyName ("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }

        [JsonPropertyName ("n")]
        public int? N { get; set; }

        [JsonPropertyName ("modalities")]
        public IList <string>? Modalities { get; set; }

        [JsonPropertyName ("prediction")]
        public yyGptChatPrediction? Prediction { get; set; }

        [JsonPropertyName ("audio")]
        public yyGptChatAudio? Audio { get; set; }

        [JsonPropertyName ("presence_penalty")]
        public double? PresencePenalty { get; set; }

        [JsonPropertyName ("response_format")]
        public yyGptChatResponseFormat? ResponseFormat { get; set; }

        [JsonPropertyName ("seed")]
        public int? Seed { get; set; }

        [JsonPropertyName ("service_tier")]
        public string? ServiceTier { get; set; }

        /// <summary>
        /// Must be string or IList <string> or null.
        /// </summary>
        [JsonPropertyName ("stop")]
        [JsonConverter (typeof (yyGptChatStopJsonConverter))]
        public object? Stop { get; set; }

        [JsonPropertyName ("stream")]
        public bool? Stream { get; set; }

        [JsonPropertyName ("stream_options")]
        public yyGptChatStreamOptions? StreamOptions { get; set; }

        [JsonPropertyName ("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName ("top_p")]
        public double? TopP { get; set; }

        [JsonPropertyName ("tools")]
        public IList <yyGptChatTool>? Tools { get; set; }

        /// <summary>
        /// Must be string or yyGptChatTool or null.
        /// </summary>
        [JsonPropertyName ("tool_choice")]
        [JsonConverter (typeof (yyGptChatToolJsonConverter))]
        public object? ToolChoice { get; set; }

        [JsonPropertyName ("parallel_tool_calls")]
        public bool? ParallelToolCalls { get; set; }

        [JsonPropertyName ("user")]
        public string? User { get; set; }

        /// <summary>
        /// Must be string or yyGptChatFunction or null.
        /// </summary>
        [JsonPropertyName ("function_call")]
        [JsonConverter (typeof (yyGptChatFunctionJsonConverter))]
        public object? FunctionCall { get; set; }

        [JsonPropertyName ("functions")]
        public IList <yyGptChatFunction>? Functions { get; set; }
    }
}
