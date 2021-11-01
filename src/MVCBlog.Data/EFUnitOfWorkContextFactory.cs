using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MVCBlog.Data
{
    public class EFUnitOfWorkContextFactory : IDesignTimeDbContextFactory<EFUnitOfWork>
    {
        public EFUnitOfWork CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EFUnitOfWork>();

            builder.UseSqlServer("Server=sql.athena.domainhizmetleri.com;Database=emrehamu_blog;User Id=emrehamu_bloguser;Password=%15Q9tcv; Trusted_Connection=false");
            return new EFUnitOfWork(builder.Options);
        }

    }
}