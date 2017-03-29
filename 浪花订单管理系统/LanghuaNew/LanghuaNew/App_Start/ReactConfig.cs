using React;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LanghuaNew.ReactConfig), "Configure")]

namespace LanghuaNew
{
	public static class ReactConfig
	{
		public static void Configure()
		{
            // If you want to use server-side rendering of React components, 
            // add all the necessary JavaScript files here. This includes 
            // your components as well as all of their dependencies.
            // See http://reactjs.net/ for more information. Example:
            //ReactSiteConfiguration.Configuration
            //	.AddScript("~/Scripts/First.jsx")
            //	.AddScript("~/Scripts/Second.jsx");

            // If you use an external build too (for example, Babel, Webpack,
            // Browserify or Gulp), you can improve performance by disabling 
            // ReactJS.NET's version of Babel and loading the pre-transpiled 
            // scripts. Example:
            //ReactSiteConfiguration.Configuration
            //	.SetLoadBabel(false)
            //	.AddScriptWithoutTransform("~/Scripts/bundle.server.js")

            ReactSiteConfiguration.Configuration
              .AddScript("~/Scripts/react_components/react-copy-to-clipboard.js")
              .AddScript("~/Scripts/react_components/autosuggest.js")
              .AddScript("~/Scripts/react_components/title.jsx")
              .AddScript("~/Scripts/react_components/titleRechange.jsx")
              .AddScript("~/Scripts/react_components/Mixin.jsx")
              .AddScript("~/Scripts/react_components/DatePicker.jsx")
              .AddScript("~/Scripts/react_components/PersonPicker.jsx")
              .AddScript("~/Scripts/react_components/FlightNo.jsx")
              .AddScript("~/Scripts/react_components/Hotel_name_zone_tel.jsx")
              .AddScript("~/Scripts/react_components/Date_setoutdate_returndate.jsx")
              .AddScript("~/Scripts/react_components/Flight_takeofftime_arrivaltime.jsx")
              .AddScript("~/Scripts/react_components/Flight_takeofftime_pickuptime.jsx")
              .AddScript("~/Scripts/react_components/Destination.jsx")
              .AddScript("~/Scripts/react_components/HotelName.jsx")
              .AddScript("~/Scripts/react_components/BusRoute.jsx")
              .AddScript("~/Scripts/react_components/TEXTinput.jsx")
              .AddScript("~/Scripts/react_components/SelectMenu.jsx")
              .AddScript("~/Scripts/react_components/ServiceTime.jsx")
              .AddScript("~/Scripts/react_components/allcomponents.jsx")
              .AddScript("~/Scripts/react_components/Copy.jsx")
              .AddScript("~/Scripts/react_components/BaseInfo.jsx");


        }
    }
}