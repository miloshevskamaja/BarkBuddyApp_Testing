using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace BarkBuddyApp.UITests
{


    public class Ui_DogBreeds_Selenium : UiTestBase
    {


        
        [Fact]
        public void DogBreeds_Index_Loads()
        {
            Driver.Navigate().GoToUrl(BASE_URL + "DogBreeds");

            
            var path = new Uri(Driver.Url).AbsolutePath;
            Assert.Contains("/DogBreeds", path, StringComparison.OrdinalIgnoreCase);

           
            var hasH2 = Driver.FindElements(By.CssSelector("h2")).Count > 0;
            var hasTableOrList = Driver.FindElements(By.CssSelector("table, ul, .container")).Count > 0;
            var hasCreateLink = Driver.FindElements(By.CssSelector("a[href*='/DogBreeds/Create']")).Count > 0;

            Assert.True(hasH2 || hasTableOrList || hasCreateLink,
                "Очекував барем наслов, табела/листa или „Create“ линк на DogBreeds Index.");
        }


   
    }

}
