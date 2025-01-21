using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySurveys.Server.Data.Entity;
using MySurveys.Server.Data.Models;

namespace MySurveys.Server.Data
{
    public class MySurveysDbContext : IdentityDbContext<IdentityUser>
    {
        public virtual DbSet<HtmlHeaderEntity>? HtmlHeaders { get; set; }
        public virtual DbSet<OptionChoicesEntity>? OptionChoices { get; set; }
        public virtual DbSet<OptionImageEntity>? OptionImage { get; set; }
        public virtual DbSet<QuestionEntity>? Question { get; set; }
        public virtual DbSet<SurveyEntity>? Surveys { get; set; }
        public virtual DbSet<AnswerEntity>? Answers { get; set; }
        public MySurveysDbContext(DbContextOptions<MySurveysDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<HtmlHeaderEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.HtmlContent).HasMaxLength(2048);
            });

            builder.Entity<OptionChoicesEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SurveyId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Option).HasMaxLength(256).IsRequired();
            });

            builder.Entity<OptionImageEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Path).HasMaxLength(256).IsRequired();
                entity.Property(e => e.Width).IsRequired();
                entity.Property(e => e.Height).IsRequired();

            });

            builder.Entity<QuestionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SurveyId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.QuestionTitle).HasMaxLength(256).IsRequired();
                entity.Property(e => e.Type).IsRequired();
            });

            builder.Entity<SurveyEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
            });

            builder.Entity<AnswerEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SurveyId).IsRequired();
                entity.Property(e => e.UserName).HasMaxLength(50);
                entity.Property(e => e.Answers).HasMaxLength(2048).IsRequired();
            });

            base.OnModelCreating(builder);
        }
    }
}