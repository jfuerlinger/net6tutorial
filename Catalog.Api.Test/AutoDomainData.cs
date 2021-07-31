using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Catalog.Core.Repositories;
using Catalog.Persistence.InMemory;

namespace Catalog.Api.Test
{
    public class AutoDomainData : AutoDataAttribute
    {
        public AutoDomainData()
            : base(() =>
            {
                var fixture = new Fixture();
                fixture.Register<IItemsRepository>(() => new InMemItemsRepository());
                fixture.Customize(new AutoMoqCustomization());
                return fixture;
            })
        { }
    }
}
