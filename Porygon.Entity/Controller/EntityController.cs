﻿using Microsoft.AspNetCore.Mvc;
using Porygon.Entity.Interfaces;
using Porygon.Entity.Manager;

namespace Porygon.Entity.Controller
{
    public abstract class EntityController : EntityController<PoryEntity, PoryEntity, EntityFilter, EntityManager>
    {
        protected EntityController(EntityManager manager) : base(manager)
        {
        }
    }

    public abstract class EntityController<T, TManager> : EntityController<T, T, EntityFilter, TManager>
        where T : PoryEntity
        where TManager : IEntityManager<T, EntityFilter, T>
    {
        protected EntityController(TManager manager) : base(manager)
        {
        }
    }

    public abstract class EntityController<T, TFilter, TManager> : EntityController<T, T, TFilter, TManager>
        where T : PoryEntity
        where TFilter : EntityFilter, new()
        where TManager : IEntityManager<T, TFilter, T>
    {
        protected EntityController(TManager manager) : base(manager)
        {
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public abstract class EntityController<T, TModel, TFilter, TManager> : ControllerBase
    where T : PoryEntity
    where TModel : T
    where TFilter : EntityFilter, new()
    where TManager : IEntityManager<T, TFilter, TModel>
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
                T? result = await Manager.Create(model);
                return result != null ? Ok(result!) : BadRequest(null);
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
                T? result = await Manager.Update(model);
                return result != null ? Ok(result!) : BadRequest(null);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                return await Manager.Delete(id) > 0 ? Ok() : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [HttpGet("{id?}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                TModel? result = await Manager.GetEnriched(id);
                return result != null ? Ok(result) : BadRequest(null);
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
                IEnumerable<TModel> result = await Manager.GetAllEnriched();
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
                var results = await Manager.SearchEnriched(filter ?? new TFilter());
                return Ok(results);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [HttpGet("lite/{id?}")]
        public async Task<IActionResult> GetLite(Guid id)
        {
            try
            {
                TModel? result = await Manager.Get(id);
                return result != null ? Ok(result) : BadRequest();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("lite")]
        public async Task<IActionResult> GetLite()
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

        [HttpPost("search/lite")]
        public async Task<IActionResult> SearchLite(TFilter filter)
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
