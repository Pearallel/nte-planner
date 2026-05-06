using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json; // 1. Add this using directive

namespace nte_planner.Models;

[Table("todos")]
public class Todo : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("task_name")]
    public string? TaskName { get; set; }

    [Column("is_complete")]
    public bool IsComplete { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // 2. Add JsonIgnore so Supabase skips this during Upsert/Insert
    [JsonIgnore]
    public bool IsSynced { get; set; } = false;
}