using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Dto
{
    public class DesignTemplateDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
    }
}
