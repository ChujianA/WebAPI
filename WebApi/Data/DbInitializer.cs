using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class DbInitializer
    {
        public static void Initialize(APIDbContext dbContenxt)
        {
            //确定数据库存在，如果存在不做任何操作，如果不存在，则创建，如果存在，要确定与数据库模型兼容
            dbContenxt.Database.EnsureCreated();

            //在此初始化默认数据
            //todo
        }
    }
}
