// File: src/Application/DTOs/Classes/ClassDto.cs
using System;
namespace Application.DTOs.Classes
{
    public class ClassDto
    {
        public string ClassId { get; set; } = string.Empty;
        public string BranchId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
    }
}