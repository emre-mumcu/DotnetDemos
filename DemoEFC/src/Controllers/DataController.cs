using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.App_Data.Context;
using src.App_Data.Entities;
using src.App_Lib;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        readonly AppDbContext _appDbContext;

        public DataController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("[action]")] // http://localhost:5281/api/Data/Welcome
        public async Task<ActionResult> Welcome()
        {
            return await Task.Run(() => Ok(new ServiceResult()
            {
                IsSuccess = true,
                ResultCode = ServiceResultCodes.Success,
                ResultMessage = $"SUCCESS"
            }));
        }

        [HttpGet("[action]/{id:int?}")]
        public async Task<ActionResult> Iller(int? id)
        {
            var q = _appDbContext.Iller;

            if (id.HasValue) q.Where(i => i.IlId == id);

            var iller = q.ToList();

            return await Task.Run(() => Ok(new ServiceResult<List<Il>>()
            {
                IsSuccess = true,
                ResultCode = ServiceResultCodes.Success,
                ResultMessage = "İller",
                ResultData = iller
            }));
        }

        [HttpGet("[action]/{id:int?}")]
        public async Task<ActionResult> IllerIlceler([FromRoute(Name = "id")] int? IlId)
        {
            var q = _appDbContext.Iller.Include(x => x.Ilceler);
            
            if(IlId.HasValue) q.Where(i => i.IlId == IlId);

            var illerilceler = q.ToList();

            return await Task.Run(() => Ok(new ServiceResult<List<Il>>()
            {
                IsSuccess = true,
                ResultCode = ServiceResultCodes.Success,
                ResultMessage = "İller (İlçeler ile birlikte)",
                ResultData = illerilceler
            }));
        }

        [HttpGet("[action]/{id:int?}")]
        public async Task<ActionResult> Advanced_RawSql(int? id)
        {
            // https://learn.microsoft.com/en-us/ef/core/querying/sql-queries
            // https://www.learnentityframeworkcore.com/raw-sql/from-sql

            // Querying scalar (non-entity) types
            // While FromSql is useful for querying entities defined in your model, SqlQuery allows you to easily query for scalar, non-entity types via SQL, without needing to drop down to lower-level data access APIs.
            // var ids = context.Database.SqlQuery<int>($"SELECT [BlogId] FROM [Blogs]").ToList();

            // Executing non-querying SQL
            // In some scenarios, it may be necessary to execute SQL which does not return any data, typically for modifying data in the database or calling a stored procedure which doesn't return any result sets. This can be done via ExecuteSql:
            // var rowsModified = context.Database.ExecuteSql($"UPDATE [Blogs] SET [Url] = NULL");

            List<State> states = new List<State>();

            if (id.HasValue)
            {
                states = _appDbContext.States.FromSqlInterpolated($"SELECT * FROM States where Id={id}").ToList();
            }
            else
            {
                // The FromSql and FromSqlInterpolated methods are safe against SQL injection, and always integrate parameter data as a separate SQL parameter. However, the FromSqlRaw method can be vulnerable to SQL injection attacks, if improperly used.

                // Queries that use FromSql or FromSqlRaw follow the exact same change tracking rules as any other LINQ query in EF Core. 

                // FromSql can only be used directly on a DbSet. It cannot be composed over an arbitrary LINQ query.
                // FromSql was introduced in EF Core 7.0. When using older versions, use FromSqlInterpolated instead.
                // .FromSql($"SELECT * FROM dbo.Blogs")
                // .FromSql($"EXECUTE dbo.GetMostPopularBlogs")
                // .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
                // .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                states = _appDbContext.States.FromSql($"SELECT * FROM States").ToList();

                // Allows dynamically constructed sql
                // FromSqlRaw method can be vulnerable to SQL injection attacks
                // .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
                // .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {propertyName} = {propertyValue}")
                states = _appDbContext.States.FromSqlRaw("SELECT * FROM States").ToList();
                
            }

            return await Task.Run(() => Ok(ServiceResults.Success(states)));
        }

    }
}
