using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Dto
{
    public class DesignedImageDto
    {
        public int? Id { get; set; }
        //public int UserId { get; set; }
        public int TemplateId { get; set; }
        public int ImageId { get; set; }
    }
}
