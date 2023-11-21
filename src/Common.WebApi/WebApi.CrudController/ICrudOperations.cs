namespace Ploch.Common.WebApi.CrudController;

public interface ICrudOperations<TModel, TKey>
{
    public TModel Get(TKey id);

    public IEnumerable<TModel> GetAll();

    public void Add(TModel model);

    public void Update(TKey id, TModel model);

    public void Delete(TKey id);
}