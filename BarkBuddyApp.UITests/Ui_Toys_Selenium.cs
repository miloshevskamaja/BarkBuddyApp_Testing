using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarkBuddyApp.UITests
{
    public class Ui_Toys_Selenium : UiTestBase
    {


        [Fact]
        public void DogBreeds_Index_Loads()
        {
            Driver.Navigate().GoToUrl(BASE_URL + "Toys");


            var path = new Uri(Driver.Url).AbsolutePath;
            Assert.Contains("/Toys", path, StringComparison.OrdinalIgnoreCase);


            var hasH2 = Driver.FindElements(By.CssSelector("h2")).Count > 0;
            var hasTableOrList = Driver.FindElements(By.CssSelector("table, ul, .container")).Count > 0;
            var hasCreateLink = Driver.FindElements(By.CssSelector("a[href*='/Toys/Create']")).Count > 0;

            Assert.True(hasH2 || hasTableOrList || hasCreateLink,
                "Очекував барем наслов, табела/листa или „Create“ линк на Toys Index.");
        }
    }
}
