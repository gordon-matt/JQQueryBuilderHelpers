using System;
using Extenso.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Data.Domain
{
    public class SavedQuery : BaseEntity<Guid>
    {
        public SavedQueryEntityType Type { get; set; }

        public string Name { get; set; }

        public string Query { get; set; }
    }

    public enum SavedQueryEntityType : byte
    {
        Person = 1,

        // TODO: Add others here
    }

    public class SavedQueryMap : IEntityTypeConfiguration<SavedQuery>
    {
        public void Configure(EntityTypeBuilder<SavedQuery> builder)
        {
            builder.ToTable("SavedQueries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(128).IsUnicode(true);
            builder.Property(x => x.Query).IsUnicode(true);
        }
    }
}