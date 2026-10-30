namespace ParaYolYonetici.Core.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? KullaniciId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public Guid EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime Tarih { get; set; }
        public string IPAddress { get; set; }
    }
}
