using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BarkBuddyApp.UITests
{

    public abstract class UiTestBase : IDisposable
    {
        protected readonly IWebDriver Driver;

        protected const string BASE_URL = "https://localhost:44373/";

        protected UiTestBase()
        {
            var opts = new ChromeOptions();
           
            opts.AddArgument("--headless=new");
            opts.AddArgument("--window-size=1400,900");
            opts.AddArgument("--disable-gpu");
            opts.AddArgument("--no-sandbox");
            Driver = new ChromeDriver(opts);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        public void Dispose()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }

}
