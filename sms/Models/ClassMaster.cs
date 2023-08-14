namespace sms.Models
{
    public class ClassMaster
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public bool IsDeleted;
    }
}
