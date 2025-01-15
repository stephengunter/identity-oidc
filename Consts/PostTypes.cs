using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Consts;

public class PostTypes
{
   public static string Event = new Event().GetType().Name;
   public static string Tasks = new Tasks().GetType().Name;
   public static string Reference = new Reference().GetType().Name;
   public static string Item = new Item().GetType().Name;
   public static string Article = new Article().GetType().Name;
}