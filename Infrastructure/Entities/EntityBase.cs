using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Interfaces;

namespace Infrastructure.Entities;
public abstract class EntityBase : IAggregateRoot
{
   [Key]
   public int Id { get; set; }


   public object GetId() => Id;
}
