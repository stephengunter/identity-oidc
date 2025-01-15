using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Interfaces;

namespace Infrastructure.Entities;
public abstract class EntityBaseView
{
   public int Id { get; set; }
}
