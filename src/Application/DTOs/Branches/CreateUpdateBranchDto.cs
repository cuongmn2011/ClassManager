// File: src/Application/DTOs/Branches/CreateUpdateBranchDto.cs
namespace Application.DTOs.Branches
{
    public class CreateUpdateBranchDto
    {
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
    }
}