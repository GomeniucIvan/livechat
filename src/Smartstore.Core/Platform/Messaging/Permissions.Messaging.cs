namespace Smartstore.Core.Security
{
    public static partial class Permissions
    {
        public static partial class Configuration
        {
            public static class EmailAccount
            {
                public const string Self = "configuration.emailaccount";
                public const string Read = "configuration.emailaccount.read";
                public const string Update = "configuration.emailaccount.update";
                public const string Create = "configuration.emailaccount.create";
                public const string Delete = "configuration.emailaccount.delete";
            }
        }

        public static partial class System
        {
            public static class Message
            {
                public const string Self = "system.message";
                public const string Read = "system.message.read";
                public const string Update = "system.message.update";
                public const string Create = "system.message.create";
                public const string Delete = "system.message.delete";
                public const string Send = "system.message.send";
            }
        }
    }
}
