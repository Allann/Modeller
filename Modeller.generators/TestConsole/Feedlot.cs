using Modeller.Models;

namespace TestConsole
{
    internal class Feedlot
    {
        public Module GetFeedlot()
        {
            return Modeller.Fluent.Module
                .Create("Feedlot")
                .CompanyName("Jbssa")
                .AddModel("Animal")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("AnimalStatus").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("AnimalSexId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("DamBreedId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("SireBreedId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("Barcode").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("Teeth").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("Weight").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("EuEligibilityStatus").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("LifetimeTraceabilityStatus").DataType(DataTypes.Number).Nullable(false).Build
                    .AddField("NlisId").MaxLength(16).Build
                    .AddField("Rfid").MaxLength(16).Build
                    .AddField("TagColourId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("TagNumber").DataType(DataTypes.Number).Nullable(true).Build
                    .AddField("TagMasterSiteId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("InductedAtSiteId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddRelationship()
                        .Relate("Animal.Id", "AnimalDeath.AnimalId")
                        .Build
                    .Build
                .AddModel("AnimalDeath")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("DeathDate").DataType(DataTypes.DateTimeOffset).Build
                    .AddField("DeathPlaceId").DataType(DataTypes.UniqueIdentifier).Build
                    .AddField("LotId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("PenId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("AnimalId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("DeathCauseId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("DeathSymptomId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("PostMortemPersonId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("AdditionalNotes").DataType(DataTypes.String).MaxLength(500).Nullable(true).Build
                    .AddField("IsCancelled").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddRelationship()
                        .Relate("Animal.Id", "AnimalDeath.AnimalId")
                        .Build
                    .Build
                .AddModel("AnimalLot")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("EntryDate").DataType(DataTypes.DateTimeOffset).Nullable(false).Build
                    .AddField("ExitDate").DataType(DataTypes.DateTimeOffset).Nullable(true).Build
                    .AddField("AnimalId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("EntryLivestockMovementId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("LotId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("ExitLivestockMovementId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddField("IsPending").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddField("IsCurrent").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddField("IsCancelled").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddRelationship()
                        .Relate("Animal.Id", "AnimalDeath.AnimalId")
                        .Build
                    .Build
                .AddModel("AnimalPen")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("IsCancelled").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddField("IsCurrent").DataType(DataTypes.Bool).Nullable(false).Build
                    .AddField("EntryDate").DataType(DataTypes.DateTimeOffset).Nullable(false).Build
                    .AddField("ExitDate").DataType(DataTypes.DateTimeOffset).Nullable(true).Build
                    .AddField("AnimalId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("EntryLivestockMovementId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("PenId").DataType(DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddField("ExitLivestockMovementId").DataType(DataTypes.UniqueIdentifier).Nullable(true).Build
                    .AddRelationship()
                        .Relate("Animal.Id", "AnimalDeath.AnimalId")
                        .Build
                    .Build
                .Build;
        }
    }
}