using AsecnaGmao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AsecnaGmao.Infrastructure.Persistence
{
    public class GmaoDbContext : DbContext
    {
        public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
        public DbSet<GroupeElectrogene> Groupes { get; set; } = null!;
        public DbSet<PieceDeRechange> Pieces { get; set; } = null!;
        public DbSet<BonIntervention> Bons { get; set; } = null!;
        public DbSet<ActionAMener> ActionsAMener { get; set; } = null!;
        public DbSet<LogTransaction> AuditLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=gmao_asecna.db");
        }

        public int SaveChanges(int currentUserId)
        {
            TrackAuditEntries(currentUserId);
            return base.SaveChanges();
        }

        private void TrackAuditEntries(int userId)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not LogTransaction &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
            {
                var auditLog = new LogTransaction
                {
                    TableNom = entry.Entity.GetType().Name,
                    UtilisateurId = userId,
                    DateHeure = DateTime.UtcNow
                };

                var originalValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditLog.Action = "INSERT";
                        foreach (var prop in entry.Properties)
                        {
                            newValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                        auditLog.ValeursNouvelles = JsonConvert.SerializeObject(newValues);
                        break;

                    case EntityState.Deleted:
                        auditLog.Action = "DELETE";
                        foreach (var prop in entry.Properties)
                        {
                            originalValues[prop.Metadata.Name] = prop.OriginalValue;
                        }
                        auditLog.ValeursAnciennes = JsonConvert.SerializeObject(originalValues);
                        break;

                    case EntityState.Modified:
                        auditLog.Action = "UPDATE";
                        foreach (var prop in entry.Properties)
                        {
                            if (prop.IsModified)
                            {
                                originalValues[prop.Metadata.Name] = prop.OriginalValue;
                                newValues[prop.Metadata.Name] = prop.CurrentValue;
                            }
                        }
                        auditLog.ValeursAnciennes = JsonConvert.SerializeObject(originalValues);
                        auditLog.ValeursNouvelles = JsonConvert.SerializeObject(newValues);
                        break;
                }

                AuditLogs.Add(auditLog);
            }
        }

        public void SeedData(string groupesCsvPath, string piecesCsvPath)
        {
            Database.EnsureCreated();

            if (!Utilisateurs.Any())
            {
                Utilisateurs.AddRange(
                    new Utilisateur { Login = "admin", Role = "Administrateur", PasswordHash = "hash_admin" },
                    new Utilisateur { Login = "tech1", Role = "Technicien", PasswordHash = "hash_tech" }
                );
                SaveChanges(); // We don't track audit logs for initial users setup (or we could pass 0)
            }

            if (!Groupes.Any() && File.Exists(groupesCsvPath))
            {
                var lines = File.ReadAllLines(groupesCsvPath).Skip(1);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    if (parts.Length >= 5)
                    {
                        Groupes.Add(new GroupeElectrogene
                        {
                            ReferenceAsecna = parts[0],
                            Modele = parts[1],
                            CompteurHeures = double.Parse(parts[2], CultureInfo.InvariantCulture),
                            DerniereMaintenanceHeures = double.Parse(parts[3], CultureInfo.InvariantCulture),
                            CycleSeuilHeures = double.Parse(parts[4], CultureInfo.InvariantCulture)
                        });
                    }
                }
                SaveChanges();
            }

            if (!Pieces.Any() && File.Exists(piecesCsvPath))
            {
                var lines = File.ReadAllLines(piecesCsvPath).Skip(1);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        Pieces.Add(new PieceDeRechange
                        {
                            Designation = parts[0],
                            QuantiteEnStock = int.Parse(parts[1], CultureInfo.InvariantCulture),
                            SeuilMinimum = int.Parse(parts[2], CultureInfo.InvariantCulture)
                        });
                    }
                }
                SaveChanges();
            }
        }
    }
}
