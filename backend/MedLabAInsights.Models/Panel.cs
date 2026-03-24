using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Panel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PanelId { get; set; }

    public string PanelName { get; set; }
    public string PanelCode { get; set; }
}