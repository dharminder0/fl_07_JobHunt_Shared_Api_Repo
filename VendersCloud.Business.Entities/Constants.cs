namespace Prompt.Engine.Business.Entities
{
    public class Setting {
        public string Key { get; set; }
        public string DefaultValue { get; set; }
    }

    public class DefaultSettings {
        public const string LanguageCode = "en-US";
        public const string LanguageName = "English";
        public const string LanguageHeader = "lang";
        public const string BaseClientCode = "BaseClient";
        public const string FreeChatFlow = "free_chat";
        public const string BlobStorage = "BlobStorage";
    }
}
