using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public interface IBasePost
{
   string Title { get; set; }
   string? Content { get; set; }
}


