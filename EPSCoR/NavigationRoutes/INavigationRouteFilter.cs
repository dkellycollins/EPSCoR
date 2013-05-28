/// This file was provided by bootstrap mvc framework. See here: https://github.com/erichexter/twitter.bootstrap.mvc 

using System;
using System.Web.Routing;

namespace NavigationRoutes
{
    public interface INavigationRouteFilter
    {
        bool  ShouldRemove(Route navigationRoutes);
    }
}
