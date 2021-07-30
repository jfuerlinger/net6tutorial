using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Catalog.Api.Test
{
    public class AutoDomainData : AutoDataAttribute
    {
        public AutoDomainData()
            : base(() =>
            {
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization());
                return fixture;
            })
        {

        }
    }
}
