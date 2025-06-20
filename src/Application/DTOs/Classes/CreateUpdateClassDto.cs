// File: src/Application/DTOs/Classes/CreateUpdateClassDto.cs
namespace Application.DTOs.Classes
{
    public class CreateUpdateClassDto
    {
        public string BranchId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
    }
}