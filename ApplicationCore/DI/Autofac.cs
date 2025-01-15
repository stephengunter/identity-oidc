using Autofac;
using Autofac.Core.Activators.Reflection;
using System.Reflection;
using ApplicationCore.DataAccess;

namespace ApplicationCore.DI;

public class ApplicationCoreModule : Autofac.Module
{
   protected override void Load(ContainerBuilder builder)
   {
      builder.RegisterGeneric(typeof(DefaultRepository<>)).As(typeof(IDefaultRepository<>)).InstancePerLifetimeScope();


      builder.RegisterAssemblyTypes(GetAssemblyByName("ApplicationCore"))
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
   }

   public static Assembly GetAssemblyByName(String AssemblyName) => Assembly.Load(AssemblyName);

}



public class InternalConstructorFinder : IConstructorFinder
{
   public ConstructorInfo[] FindConstructors(Type targetType)
         => targetType.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
}
