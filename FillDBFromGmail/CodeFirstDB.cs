using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace FillDBFromGmail
{
    [Table("GmailMessages")]
    public class EMessage
    {
        [Key]
        public string MessageId { get; set; }
        public int Size { get; set; }
        public string Snippet { get; set; }
    }

    public class MessageContext : DbContext
    {
        public DbSet<EMessage> Messages { get; set; }
    }

}
