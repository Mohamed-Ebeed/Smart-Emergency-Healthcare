
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Infrastructure.Identity;

namespace SmartEmergency.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Recipient User ID
            builder.Property(x => x.RecipientUserId)
                .HasMaxLength(450);

            // Channel Conversion
            builder.Property(x => x.Channel)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Application User Relationship
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Emergency Contact Relationship
            builder.HasOne(x => x.EmergencyContact)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.EmergencyContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Emergency Request Relationship
            builder.HasOne(x => x.EmergencyRequest)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Recipient Check Constraint
            builder.ToTable("Notifications", table =>
                table.HasCheckConstraint(
                    "CK_Notifications_Recipient",
                    "([RecipientUserId] IS NOT NULL AND [EmergencyContactId] IS NULL) OR ([RecipientUserId] IS NULL AND [EmergencyContactId] IS NOT NULL)"
                ));
        }
    }
}
