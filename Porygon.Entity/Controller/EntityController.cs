using Microsoft.AspNetCore.Mvc;
using Porygon.Entity.Manager;

namespace Porygon.Entity.Controller
{
    public abstract class EntityController<T, TManager> : EntityController<T, Guid, T, EntityFilter, TManager>
        where T : PoryEntity<Guid>
        where TManager : IEntityManager<T, Guid, EntityFilter, T>
    {
        protected EntityController(TManager manager) : base(manager)
        {
        }
    }

    public abstract class EntityController<T, TFilter, TManager> : EntityController<T, Guid, T, TFilter, TManager>
        where T : PoryEntity<Guid>
        where TFilter : EntityFilter, new()
        where TManager : IEntityManager<T, Guid, TFilter, T>
    {
        protected EntityController(TManager manager) : base(manager)
        {
        }
    }

    public abstract class EntityController<T, TModel, TFilter, TManager> : EntityController<T, Guid, TModel, TFilter, TManager>
        where T : PoryEntity<Guid>
        where TModel : class
        where TFilter : EntityFilter, new()
        where TManager : IEntityManager<T, Guid, TFilter, TModel>
    {
        protected EntityController(TManager manager) : base(manager)
        {
        }
    }


    [ApiController]
    [Route("api/[controller]")]
    public abstract class EntityController<T, TKey, TModel, TFilter, TManager> : ControllerBase
    where T : PoryEntity<TKey>
    where TModel : class
    where TFilter : EntityFilter, new()
    where TManager : IEntityManager<T, TKey, TFilter, TModel>
    {
        protected TManager Manager;

        public EntityController(TManager manager)
        {
            Manager = manager;
        }


        [HttpPut]
        public async Task<IActionResult> Create(TModel model)
        {
            try
            {
                T result = await Manager.Create(model);
                return result != null ? Ok(result) : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(TModel model)
        {
            try
            {
                T result = await Manager.Update(model);
                return result != null ? Ok(result) : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id?}")]
        public IActionResult Delete(TKey id)
        {
            try
            {
                return Manager.Delete(id) > 0 ? Ok() : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [HttpGet("{id?}")]
        public async Task<IActionResult> Get(TKey id)
        {
            try
            {
                TModel result = await Manager.Get(id);
                return result != null ? Ok(result) : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                IEnumerable<TModel> result = await Manager.GetAll();
                return Ok(result);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(TFilter filter)
        {
            try
            {
                var results = await Manager.Search(filter ?? new TFilter());
                return Ok(results);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
