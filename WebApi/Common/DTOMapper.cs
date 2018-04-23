
using System.Collections.Generic;
using AutoMapper;
using WebApi.Common.Interfaces;

namespace WebApi.Common
{
    public class DTOMapper:IDTOMapper
    {
        private MapperConfiguration config { get; set; } = null;
        public TResult AutoMapper<TSource, TResult>(TSource sourceModel)
        {
           // Registor<TSource, TResult>();
            var result = Mapper.Map<TSource,TResult>(sourceModel);
            return result;
        }

        public List<TResult> AutoMapper<TSource, TResult>(List<TSource> sourceList)
        {
            InstanceRegistor<TSource, TResult>();
            if (config == null)
            {
                return default(List<TResult>);
            }
            else
            {
                var mapper = config.CreateMapper();
                var result = mapper.Map<List<TSource>, List<TResult>>(sourceList);
                return result;
            }
        }

        //实例化方式
        public void InstanceRegistor<TSource, TResult>()
        {
            config = new MapperConfiguration(cfg => { cfg.CreateMap<TSource, TResult>(); });

        }
    }
}
