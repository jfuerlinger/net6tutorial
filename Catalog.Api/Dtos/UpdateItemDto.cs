using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Api.Dtos
{
    [ExcludeFromCodeCoverage]
    public record UpdateItemDto
    {
        [Required]
        public string Name { get; init; }

        [Required]
        [Range(1, 1000)]
        public decimal Price { get; init; }
    }
}