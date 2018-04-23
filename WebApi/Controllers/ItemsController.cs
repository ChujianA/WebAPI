using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;
using WebApi.Common.Interfaces;
using WebApi.Data;
using WebApi.Data.Items;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Items")]
    public class ItemsController : Controller
    {
        private APIDbContext dbContext;
        private IDTOMapper dtoMapper;

        public ItemsController(APIDbContext DbContext, IDTOMapper mapper)
        {
            dbContext = DbContext;
            dtoMapper = mapper;
        }
        // GET: api/Items
        [Authorize]
        [HttpGet]
        public List<ItemsViewModel> Get()
        {
            var list = dbContext.Items.ToList();
            var result=dtoMapper.AutoMapper<Item, ItemsViewModel>(list);
            return result;              
        }

        // GET: api/Items/5
        [HttpGet("{id}", Name = "Get")]
        public ItemsViewModel Get(int id)
        {
           var item= dbContext.Items.Where(x => x.Id == id).FirstOrDefault();
            var ItemViewModel = dtoMapper.AutoMapper<Item, ItemsViewModel>(item);
            return ItemViewModel;
        }
        
        // POST: api/Items
        //[HttpPost("Add")]
        [HttpPost]
        [EnableCors("any")]
        public int  Post([FromBody] Item entity)
        {
             //var   entity=new Item();

            entity.LastModifiedDate = DateTime.Now;
            entity.CreatedDate = DateTime.Now;
            entity.Flags = 1;
            entity.ViewCount = 1;
            dbContext.Items.Add(entity);
           int result=dbContext.SaveChanges();
            return result;
        }
        
        // PUT: api/Items/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
