namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class LinkBuilder
    {
        private readonly Repository.Objects.Link _link;

        public LinkBuilder()
        {
            _link = new Repository.Objects.Link();
        }

        public LinkBuilder WithId(short id)
        {
            _link.ID = id;

            return this;
        }

        public Repository.Objects.Link Build()
        {
            return _link;
        }
    }
}