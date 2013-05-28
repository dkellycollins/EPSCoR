using System.Web.Mvc;
using BootstrapSupport;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Contains methods for working with bootstrap's alerts.
    /// </summary>
    public class BootstrapBaseController: Controller
    {
        /// <summary>
        /// Displays an attention message on the page returned.
        /// </summary>
        /// <param name="message">Message to display</param>
        public void DisplayAttention(string message)
        {
            TempData.Add(Alerts.ATTENTION, message);
        }

        /// <summary>
        /// Displays a success message on the page returned.
        /// </summary>
        /// <param name="message">Message to display</param>
        public void DisplaySuccess(string message)
        {
            TempData.Add(Alerts.SUCCESS, message);
        }

        /// <summary>
        /// Displays an info message on the page returned.
        /// </summary>
        /// <param name="message">Message to display</param>
        public void DisplayInformation(string message)
        {
            TempData.Add(Alerts.INFORMATION, message);
        }

        /// <summary>
        /// Displays an error message on the page returned.
        /// </summary>
        /// <param name="message">Message to display</param>
        public void DisplayError(string message)
        {
            TempData.Add(Alerts.ERROR, message);
        }
    }
}
