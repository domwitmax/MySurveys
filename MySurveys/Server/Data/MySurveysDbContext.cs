using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MySurveys.Server.Data
{
    public class MySurveysDbContext : IdentityDbContext<IdentityUser>
    {
        public MySurveysDbContext(DbContextOptions<MySurveysDbContext> options) : base(options) { }
    }
}
