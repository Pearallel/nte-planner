using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace nte_planner.Models
{
    [Table("rarity")]
    [System.ComponentModel.DataAnnotations.Schema.Table("rarity")]
    public class Rarity : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }
    }

    [Table("combat_role")]
    [System.ComponentModel.DataAnnotations.Schema.Table("combat_role")]
    public class CombatRole : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }
    }

    [Table("element")]
    [System.ComponentModel.DataAnnotations.Schema.Table("element")]
    public class Element : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }
    }

    [Table("arc_compatability")]
    [System.ComponentModel.DataAnnotations.Schema.Table("arc_compatability")]
    public class ArcCompatability : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }
    }

    [Table("esper")]
    [System.ComponentModel.DataAnnotations.Schema.Table("esper")]
    public class Esper : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("rarity_id")]
        [ForeignKey(nameof(Rarity))]
        public int RarityId { get; set; }

        [Column("combat_role_id")]
        [ForeignKey(nameof(CombatRole))]
        public int CombatRoleId { get; set; }

        [Column("element_id")]
        [ForeignKey(nameof(Element))]
        public int ElementId { get; set; }

        [Column("arc_compatability_id")]
        [ForeignKey(nameof(ArcCompatability))]
        public int ArcCompatabilityId { get; set; }

        // Navigation Properties
        public Rarity? Rarity { get; set; }
        public CombatRole? CombatRole { get; set; }
        public Element? Element { get; set; }
        public ArcCompatability? ArcCompatability { get; set; }
    }

    [Table("arc")]
    [System.ComponentModel.DataAnnotations.Schema.Table("arc")]
    public class Arc : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("rarity_id")]
        [ForeignKey(nameof(Rarity))]
        public int RarityId { get; set; }

        [Column("arc_compatability_id")]
        [ForeignKey(nameof(ArcCompatability))]
        public int ArcCompatabilityId { get; set; }

        // Navigation Properties
        public Rarity? Rarity { get; set; }
        public ArcCompatability? ArcCompatability { get; set; }
    }

    [Table("cartridge")]
    [System.ComponentModel.DataAnnotations.Schema.Table("cartridge")]
    public class Cartridge : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("rarity_id")]
        [ForeignKey(nameof(Rarity))]
        public int RarityId { get; set; }

        public Rarity? Rarity { get; set; }
    }

    [Table("module")]
    [System.ComponentModel.DataAnnotations.Schema.Table("module")]
    public class Module : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("rarity_id")]
        [ForeignKey(nameof(Rarity))]
        public int RarityId { get; set; }

        public Rarity? Rarity { get; set; }
    }

    [Table("hunter_level")]
    [System.ComponentModel.DataAnnotations.Schema.Table("hunter_level")]
    public class HunterLevel : BaseModel
    {
        [Column("level")]
        [Key]
        [PrimaryKey("level", false)]
        public int Level { get; set; }
    }

    [Table("profile")]
    [System.ComponentModel.DataAnnotations.Schema.Table("profile")]
    public class Profile : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public string? Id { get; set; }

        [Column("character_pixels")]
        public int CharacterPixels { get; set; }

        [Column("character_pixel_capacity")]
        public int CharacterPixelCapacity { get; set; }

        [Column("hunter_level_level")]
        [ForeignKey(nameof(HunterLevel))]
        public int HunterLevelLevel { get; set; }

        public HunterLevel? HunterLevel { get; set; }
    }

    [Table("owned_esper")]
    [System.ComponentModel.DataAnnotations.Schema.Table("owned_esper")]
    public class OwnedEsper : BaseModel
    {
        [Column("esper_id")]
        [Key]
        [PrimaryKey("esper_id", false)]
        [ForeignKey(nameof(Esper))]
        public int EsperId { get; set; }

        [Column("profile_id")]
        [Key]
        [PrimaryKey("profile_id", false)]
        [ForeignKey(nameof(Profile))]
        public string? ProfileId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("current_level")]
        public int CurrentLevel { get; set; }

        [Column("target_level")]
        public int TargetLevel { get; set; }

        [Column("current_asc_level")]
        public int CurrentAscLevel { get; set; }

        [Column("target_asc_level")]
        public int TargetAscLevel { get; set; }

        [Column("current_basic_level")]
        public int CurrentBasicLevel { get; set; }

        [Column("target_basic_level")]
        public int TargetBasicLevel { get; set; }

        [Column("current_skill_level")]
        public int CurrentSkillLevel { get; set; }

        [Column("target_skill_level")]
        public int TargetSkillLevel { get; set; }

        [Column("current_ultimate_level")]
        public int CurrentUltimateLevel { get; set; }

        [Column("target_ultimate_level")]
        public int TargetUltimateLevel { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        public Esper? Esper { get; set; }
        public Profile? Profile { get; set; }
    }

    [Table("owned_arc")]
    [System.ComponentModel.DataAnnotations.Schema.Table("owned_arc")]
    public class OwnedArc : BaseModel
    {
        [Column("arc_id")]
        [Key]
        [PrimaryKey("arc_id", false)]
        [ForeignKey(nameof(Arc))]
        public int ArcId { get; set; }

        [Column("profile_id")]
        [Key]
        [PrimaryKey("profile_id", false)]
        [ForeignKey(nameof(Profile))]
        public string? ProfileId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("current_level")]
        public int CurrentLevel { get; set; }

        [Column("target_level")]
        public int TargetLevel { get; set; }

        [Column("current_asc_level")]
        public int CurrentAscLevel { get; set; }

        [Column("target_asc_level")]
        public int TargetAscLevel { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        public Arc? Arc { get; set; }
        public Profile? Profile { get; set; }
    }

    [Table("owned_cartridge")]
    [System.ComponentModel.DataAnnotations.Schema.Table("owned_cartridge")]
    public class OwnedCartridge : BaseModel
    {
        [Column("cartridge_id")]
        [Key]
        [PrimaryKey("cartridge_id", false)]
        [ForeignKey(nameof(Cartridge))]
        public int CartridgeId { get; set; }

        [Column("profile_id")]
        [Key]
        [PrimaryKey("profile_id", false)]
        [ForeignKey(nameof(Profile))]
        public string? ProfileId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("current_level")]
        public int CurrentLevel { get; set; }

        [Column("target_level")]
        public int TargetLevel { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        public Cartridge? Cartridge { get; set; }
        public Profile? Profile { get; set; }
    }

    [Table("owned_module")]
    [System.ComponentModel.DataAnnotations.Schema.Table("owned_module")]
    public class OwnedModule : BaseModel
    {
        [Column("module_id")]
        [Key]
        [PrimaryKey("module_id", false)]
        [ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }

        [Column("profile_id")]
        [Key]
        [PrimaryKey("profile_id", false)]
        [ForeignKey(nameof(Profile))]
        public string? ProfileId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("current_level")]
        public int CurrentLevel { get; set; }

        [Column("target_level")]
        public int TargetLevel { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        public Module? Module { get; set; }
        public Profile? Profile { get; set; }
    }

    [Table("item")]
    [System.ComponentModel.DataAnnotations.Schema.Table("item")]
    public class Item : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("image_link")]
        public string? ImageLink { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        [Column("rarity_id")]
        [ForeignKey(nameof(Rarity))]
        public int RarityId { get; set; }

        public Rarity? Rarity { get; set; }
    }

    [Table("owned_item")]
    [System.ComponentModel.DataAnnotations.Schema.Table("owned_item")]
    public class OwnedItem : BaseModel
    {
        [Column("item_id")]
        [Key]
        [PrimaryKey("item_id", false)]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [Column("profile_id")]
        [Key]
        [PrimaryKey("profile_id", false)]
        [ForeignKey(nameof(Profile))]
        public string? ProfileId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("last_updated_at")]
        public DateTime? LastUpdatedAt { get; set; }

        public Item? Item { get; set; }
        public Profile? Profile { get; set; }
    }

    [Table("farm_option")]
    [System.ComponentModel.DataAnnotations.Schema.Table("farm_option")]
    public class FarmOption : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("character_pixel_cost")]
        public int CharacterPixelCost { get; set; }

        [Column("hunter_level_level")]
        [ForeignKey(nameof(HunterLevel))]
        public int HunterLevelLevel { get; set; }

        public HunterLevel? HunterLevel { get; set; }
    }

    [Table("farm_option_avg_reward")]
    [System.ComponentModel.DataAnnotations.Schema.Table("farm_option_avg_reward")]
    public class FarmOptionAvgReward : BaseModel
    {
        [Column("farm_option_id")]
        [Key]
        [PrimaryKey("farm_option_id", false)]
        [ForeignKey(nameof(FarmOption))]
        public int FarmOptionId { get; set; }

        [Column("item_id")]
        [Key]
        [PrimaryKey("item_id", false)]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        public FarmOption? FarmOption { get; set; }
        public Item? Item { get; set; }
    }

    [Table("upgrade_recipe")]
    [System.ComponentModel.DataAnnotations.Schema.Table("upgrade_recipe")]
    public class UpgradeRecipe : BaseModel
    {
        [Column("id")]
        [Key]
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("module_id")]
        [ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }

        [Column("cartridge_id")]
        [ForeignKey(nameof(Cartridge))]
        public int CartridgeId { get; set; }

        [Column("arc_asc_id")]
        [ForeignKey(nameof(ArcAsc))] // <-- FIXED: Pointing to the ArcAsc navigation property
        public int ArcAscId { get; set; }

        [Column("arc_id")]
        [ForeignKey(nameof(Arc))] // <-- FIXED: Pointing to the Arc navigation property
        public int ArcId { get; set; }

        [Column("esper_asc_id")]
        [ForeignKey(nameof(EsperAsc))] // <-- FIXED: Pointing to the EsperAsc navigation property
        public int EsperAscId { get; set; }

        [Column("esper_id")]
        [ForeignKey(nameof(Esper))] // <-- FIXED: Pointing to the Esper navigation property
        public int EsperId { get; set; }

        [Column("element_id")]
        [ForeignKey(nameof(Element))]
        public int ElementId { get; set; }

        public Module? Module { get; set; }
        public Cartridge? Cartridge { get; set; }

        public Arc? ArcAsc { get; set; } // Matches ArcAscId
        public Arc? Arc { get; set; }    // Matches ArcId

        public Esper? EsperAsc { get; set; } // Matches EsperAscId
        public Esper? Esper { get; set; }    // Matches EsperId

        public Element? Element { get; set; }
    }

    [Table("upgrade_recipe_item")]
    [System.ComponentModel.DataAnnotations.Schema.Table("upgrade_recipe_item")]
    public class UpgradeRecipeItem : BaseModel
    {
        [Column("upgrade_recipe_id")]
        [Key]
        [PrimaryKey("upgrade_recipe_id", false)]
        [ForeignKey(nameof(UpgradeRecipe))]
        public int UpgradeRecipeId { get; set; }

        [Column("item_id")]
        [Key]
        [PrimaryKey("item_id", false)]
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        public UpgradeRecipe? UpgradeRecipe { get; set; }
        public Item? Item { get; set; }
    }
}