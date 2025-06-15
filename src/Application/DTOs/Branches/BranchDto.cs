// File: src/Application/DTOs/Branches/BranchDto.cs
using System;
namespace Application.DTOs.Branches
{
    public class BranchDto
    {
        public string BranchId { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
    }
}