using CSVProcessor.Database.Services;

namespace CSVProcessor.Database
{
    public class TableDetails
    {
        public Company Company => new Company();

        public Brand Brand => new Brand();

        public Email Email => new Email();

        public Site Site => new Site();

        public SiteEnterprise SiteEnterprise => new SiteEnterprise();

        public SiteCommunity SiteCommunity => new SiteCommunity();

        public SiteXsiDetail SiteXsiDetail => new SiteXsiDetail();

        public UnitType UnitType => new UnitType();

        public SiteUnitType SiteUnitType => new SiteUnitType();

        public User User => new User();

        public UserCredential UserCredential => new UserCredential();

        public Job Job => new Job();

        public Extension Extension => new Extension();

        public InFeed InFeed => new InFeed();

        public OutFeed OutFeed => new OutFeed();
    }
}