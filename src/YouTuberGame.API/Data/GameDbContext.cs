using Microsoft.EntityFrameworkCore;
using YouTuberGame.Shared.Models;

namespace YouTuberGame.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PlayerData> PlayerData { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<PlayerCharacter> PlayerCharacters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 설정
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.LastLogin).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // PlayerData 설정
            modelBuilder.Entity<PlayerData>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                      .WithOne(u => u.PlayerData)
                      .HasForeignKey<PlayerData>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Character 설정
            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(e => e.CharacterId);
            });

            // PlayerCharacter 설정
            modelBuilder.Entity<PlayerCharacter>(entity =>
            {
                entity.HasKey(e => e.InstanceId);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.AcquiredAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Character)
                      .WithMany()
                      .HasForeignKey(e => e.CharacterId);
            });

            // 기본 캐릭터 데이터 시드
            SeedCharacters(modelBuilder);
        }

        private void SeedCharacters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>().HasData(
                // C등급 캐릭터
                new Character
                {
                    CharacterId = "char_001",
                    CharacterName = "김촬영",
                    Rarity = CharacterRarity.C,
                    Specialty = CharacterSpecialty.Filming,
                    BaseFilming = 30,
                    BaseEditing = 10,
                    BasePlanning = 10,
                    BaseDesign = 10
                },
                new Character
                {
                    CharacterId = "char_002",
                    CharacterName = "박편집",
                    Rarity = CharacterRarity.C,
                    Specialty = CharacterSpecialty.Editing,
                    BaseFilming = 10,
                    BaseEditing = 30,
                    BasePlanning = 10,
                    BaseDesign = 10
                },
                new Character
                {
                    CharacterId = "char_003",
                    CharacterName = "이기획",
                    Rarity = CharacterRarity.C,
                    Specialty = CharacterSpecialty.Planning,
                    BaseFilming = 10,
                    BaseEditing = 10,
                    BasePlanning = 30,
                    BaseDesign = 10
                },
                new Character
                {
                    CharacterId = "char_004",
                    CharacterName = "최디자인",
                    Rarity = CharacterRarity.C,
                    Specialty = CharacterSpecialty.Design,
                    BaseFilming = 10,
                    BaseEditing = 10,
                    BasePlanning = 10,
                    BaseDesign = 30
                },
                // B등급 캐릭터
                new Character
                {
                    CharacterId = "char_005",
                    CharacterName = "강PD",
                    Rarity = CharacterRarity.B,
                    Specialty = CharacterSpecialty.Planning,
                    BaseFilming = 15,
                    BaseEditing = 15,
                    BasePlanning = 40,
                    BaseDesign = 15
                },
                // A등급 캐릭터
                new Character
                {
                    CharacterId = "char_006",
                    CharacterName = "정프로",
                    Rarity = CharacterRarity.A,
                    Specialty = CharacterSpecialty.Filming,
                    BaseFilming = 50,
                    BaseEditing = 30,
                    BasePlanning = 20,
                    BaseDesign = 20,
                    PassiveSkillDesc = "촬영 품질 +10%",
                    PassiveSkillValue = 0.1f
                },
                // S등급 캐릭터
                new Character
                {
                    CharacterId = "char_007",
                    CharacterName = "천재 크리에이터",
                    Rarity = CharacterRarity.S,
                    Specialty = CharacterSpecialty.Planning,
                    BaseFilming = 40,
                    BaseEditing = 40,
                    BasePlanning = 60,
                    BaseDesign = 40,
                    PassiveSkillDesc = "전체 스탯 +15%",
                    PassiveSkillValue = 0.15f
                }
            );
        }
    }
}
