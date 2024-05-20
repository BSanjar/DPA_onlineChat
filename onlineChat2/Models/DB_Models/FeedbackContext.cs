using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace onlineChat2.Models.DB_Models;

public partial class FeedbackContext : DbContext
{
    public FeedbackContext()
    {
    }

    public FeedbackContext(DbContextOptions<FeedbackContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Themcategorye> Themcategoryes { get; set; }

    public virtual DbSet<Translation> Translations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=192.168.88.211;Port=5432;Database=feedback;Username=feedbacksys;Password=feedbacksys");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pk");

            entity.ToTable("chats");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Admin)
                .HasComment("админ кто отвечал, в случае чата кто ответил последним")
                .HasColumnType("character varying")
                .HasColumnName("admin");
            entity.Property(e => e.Answer)
                .HasComment("ответ если feedback")
                .HasColumnType("character varying")
                .HasColumnName("answer");
            entity.Property(e => e.Chat1)
                .HasComment("последний сохраненный чат html")
                .HasColumnType("character varying")
                .HasColumnName("chat");
            entity.Property(e => e.CreatedAt)
                .HasComment("время обращения, если чат то время первого сообщения")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LangMsg)
                .HasComment("язык обращения - ru, ky, en")
                .HasColumnType("character varying")
                .HasColumnName("lang_msg");
            entity.Property(e => e.MsgSource)
                .HasComment("onlinechat, feedback, telegram")
                .HasColumnType("character varying")
                .HasColumnName("msg_source");
            entity.Property(e => e.Question)
                .HasComment("вопрос если feedback")
                .HasColumnType("character varying")
                .HasColumnName("question");
            entity.Property(e => e.Status)
                .HasComment("new, waiting, closed")
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasComment("тема обращения если feedback")
                .HasColumnType("character varying")
                .HasColumnName("title");
            entity.Property(e => e.TypeTheme)
                .HasComment("юридические(jur) или техничесские(tech) вопросы")
                .HasColumnType("character varying")
                .HasColumnName("type_theme");
            entity.Property(e => e.UpdatedAt)
                .HasComment("последнее обновление")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.User)
                .HasComment("гражданин кто обращался")
                .HasColumnType("character varying")
                .HasColumnName("user");

            entity.HasOne(d => d.AdminNavigation).WithMany(p => p.ChatAdminNavigations)
                .HasForeignKey(d => d.Admin)
                .HasConstraintName("chats_fk_1");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.ChatUserNavigations)
                .HasForeignKey(d => d.User)
                .HasConstraintName("chats_fk");
        });

        modelBuilder.Entity<Themcategorye>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("themcategoryes_pk");

            entity.ToTable("themcategoryes");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasColumnType("character varying")
                .HasColumnName("category");
            entity.Property(e => e.TypeTheme)
                .HasComment("jur, tech")
                .HasColumnType("character varying")
                .HasColumnName("type_theme");
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("translations_pk");

            entity.ToTable("translations");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.En)
                .HasColumnType("character varying")
                .HasColumnName("en");
            entity.Property(e => e.Isdeleted)
                .HasColumnType("character varying")
                .HasColumnName("isdeleted");
            entity.Property(e => e.Ky)
                .HasColumnType("character varying")
                .HasColumnName("ky");
            entity.Property(e => e.Ru)
                .HasColumnType("character varying")
                .HasColumnName("ru");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pk");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.LastIp)
                .HasColumnType("character varying")
                .HasColumnName("last_ip");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasColumnType("character varying")
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasComment("admin, jur, tech, user")
                .HasColumnType("character varying")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
