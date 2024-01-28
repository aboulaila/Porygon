using Porygon.Entity.Interfaces;

namespace Porygon.Entity.Tasks
{
    public class EntitySearcher<T, TModel, TFilter, TDataManager> : EntityRetriever<T, TModel>
        where T : class, IKeyEntity<Guid>, new()
        where TFilter : EntityFilter, new()
        where TModel : T
        where TDataManager : IEntityDataManager<T, Guid, TFilter>
    {
        private TFilter? filter;
        private new TDataManager dataManager;
        public EntitySearcher(IEntityManager entityManager, TDataManager dataManager, IServiceProvider serviceProvider) : base(entityManager, serviceProvider)
        {
            this.dataManager = dataManager;
        }

        public EntitySearcher<T, TModel, TFilter, TDataManager> Filter(TFilter filter)
        {
            this.filter = filter;
            return this;
        }

        public new EntitySearcher<T, TModel, TFilter, TDataManager> ShouldEnrich()
        {
            base.ShouldEnrich();
            return this;
        }

        public async Task<List<TModel>> Search()
        {
            if (filter == null)
                throw new ArgumentNullException("Filter must be specified");

            var results = await dataManager.Search(filter!);
            return await ToViewModel(results);
        }
    }
}