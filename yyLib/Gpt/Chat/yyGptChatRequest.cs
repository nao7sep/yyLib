using System.Text.Json.Serialization;

namespace yyGptLib
{
    public class yyGptChatRequest
    {
        // Everything except function-related things.
        // https://platform.openai.com/docs/api-reference/chat/create

        // The properties are sorted in the order of the API reference.

        [JsonPropertyName ("messages")]
        public IList <yyGptChatMessage>? Messages { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

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

        [JsonPropertyName ("n")]
        public int? N { get; set; }

        [JsonPropertyName ("presence_penalty")]
        public double? PresencePenalty { get; set; }

        [JsonPropertyName ("response_format")]
        public yyGptChatRequestResponseFormat? ResponseFormat { get; set; }

        [JsonPropertyName ("seed")]
        public int? Seed { get; set; }

        [JsonPropertyName ("stop")]
        public IList <string>? Stop { get; set; }

        [JsonPropertyName ("stream")]
        public bool? Stream { get; set; }

        [JsonPropertyName ("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName ("top_p")]
        public double? TopP { get; set; }

        [JsonPropertyName ("user")]
        public string? User { get; set; }

        public yyGptChatRequest () => Model = yyGptChat.DefaultModel;

        public void AddMessage (yyGptChatMessageRole role, string content, string? name = null)
        {
            Messages ??= new List <yyGptChatMessage> ();

            Messages.Add (new yyGptChatMessage
            {
                Role = role,
                Content = content,
                Name = name
            });
        }

        public void AddSystemMessage (string content, string? name = null) =>
            AddMessage (yyGptChatMessageRole.System, content, name);

        public void AddUserMessage (string content, string? name = null) =>
            AddMessage (yyGptChatMessageRole.User, content, name);

        public void AddAssistantMessage (string content, string? name = null) =>
            AddMessage (yyGptChatMessageRole.Assistant, content, name);

        public string? GetLastMessage ()
        {
            if (Messages != null && Messages.Count > 0)
                return Messages [Messages.Count - 1].Content;

            return null;
        }

        public void RemoveLastMessage ()
        {
            if (Messages != null && Messages.Count > 0)
                Messages.RemoveAt (Messages.Count - 1);
        }
    }
}
