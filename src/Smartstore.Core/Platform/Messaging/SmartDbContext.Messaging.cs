using Smartstore.Core.Messaging;

namespace Smartstore.Core.Data
{
    public partial class SmartDbContext
    {
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public DbSet<QueuedEmailAttachment> QueuedEmailAttachments { get; set; }
    }
}