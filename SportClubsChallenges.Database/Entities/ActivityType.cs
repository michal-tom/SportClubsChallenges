namespace SportClubsChallenges.Database.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ActivityType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public byte Id { get; set; }

        public string Name { get; set; }
    }
}