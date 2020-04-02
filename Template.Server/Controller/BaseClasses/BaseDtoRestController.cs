using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Template.Db.Interfaces;
using Template.Models;

namespace Template.Server.Controller.BaseClasses {
    public abstract class BaseDtoRestController<TDto, TModel> : ControllerBase
        where TModel : ModelBase, new() where TDto : new() {
        protected readonly IRepository<TModel> _repository;
        private readonly IMapper _mapper;

        protected BaseDtoRestController(IRepository<TModel> repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public virtual IActionResult Get()
            => Ok(_repository.Get().Select(e => _mapper.Map<TDto>(e)));


        [HttpGet("{id}")]
        public virtual IActionResult Get(string id)
            => Ok(_mapper.Map<TDto>(_repository.Get(id)));


        [HttpPost]
        public virtual IActionResult Post(TDto entity) {
            var model = _mapper.Map<TModel>(entity);
            var result = _repository.Add(model);
            return Ok(_mapper.Map<TDto>(result));
        }


        [HttpPut("{id}")]
        public virtual IActionResult Put(string id, TDto entity) {
            var model = _repository.Get(id);
            var updated = _mapper.Map(entity, model);
            var result = _repository.Update(updated);
            return Ok(_mapper.Map<TDto>(result));
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(string id) {
            var entity = _repository.Get(id);
            if (entity == null)
                return NotFound();

            _repository.Delete(entity);
            return Ok();
        }
    }
}