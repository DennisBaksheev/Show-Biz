using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DBA5.Startup))]

namespace DBA5
{
   public partial class Startup
   {
      public void Configuration(IAppBuilder app)
      {
         ConfigureAuth(app);
      }
   }
}
