/// This file was provided by bootstrap mvc framework. See here: https://github.com/erichexter/twitter.bootstrap.mvc 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace NavigationRoutes
{
    public class NavigationRouteFilter : INavigationRouteFilter
    {
        public bool ShouldRemove(Route route)
        {
            return true;
        }
    }
}
