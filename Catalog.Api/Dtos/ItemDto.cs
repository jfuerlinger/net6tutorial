using System;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Api.Dtos {

    [ExcludeFromCodeCoverage]
    public record ItemDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}