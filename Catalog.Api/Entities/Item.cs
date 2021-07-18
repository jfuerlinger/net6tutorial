using System;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Api.Entities
{
    [ExcludeFromCodeCoverage]
    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}