using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations
{
    internal class PropertyMediaConfiguration : IEntityTypeConfiguration<PropertyMedia>
    {
        public void Configure(EntityTypeBuilder<PropertyMedia> builder)
        {
            builder.ToTable("PropertyMedia");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MediaUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.MediaType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(1);

            builder.HasIndex(x => new { x.PropertyAdId, x.Order })
           .IsUnique();


            builder.Property(x => x.PropertyAdId)
                .IsRequired();

            builder.HasOne(x => x.PropertyAd)
                .WithMany(p => p.Media)
                .HasForeignKey(x => x.PropertyAdId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PropertyAdId);
        }
    }
    
    }

