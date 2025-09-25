using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace BarkBuddyApp.UITests
{
    public class Ui_Auth_Selenium : UiTestBase
    {
        private static string UniqueEmail() =>
            $"ui.user.{DateTime.UtcNow:yyyyMMddHHmmssfff}@example.com";

        private const string TestPassword = "P@ssw0rd!123";

        public Ui_Auth_Selenium() : base() { } 

        private IWebElement FindField(string logicalName)
        {
            
            var el = Driver.FindElements(By.Id(logicalName)).FirstOrDefault();
            if (el != null) return el;

            el = Driver.FindElements(By.Name(logicalName)).FirstOrDefault();
            if (el != null) return el;

         
            foreach (var lbl in Driver.FindElements(By.CssSelector("label[for]")))
            {
                var forId = lbl.GetAttribute("for");
                if (!string.IsNullOrWhiteSpace(forId) &&
                    lbl.Text.IndexOf(logicalName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    el = Driver.FindElements(By.Id(forId)).FirstOrDefault();
                    if (el != null) return el;
                }
            }
            return null;
        }

        private void SetText(string logicalName, string value)
        {
            var el = FindField(logicalName);
            if (el == null) throw new NoSuchElementException($"Field '{logicalName}' not found.");
            el.Clear();
            el.SendKeys(value);
        }

        private void ClickSubmit()
        {
            var submit = Driver.FindElements(By.CssSelector("input[type=submit],button[type=submit]"))
                               .FirstOrDefault();
            if (submit == null) throw new NoSuchElementException("Submit button not found.");
            submit.Click();
        }


        private IWebElement WaitFor(Func<IWebDriver, IWebElement> condition, int timeoutSec = 15)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSec));
            return wait.Until(condition);
        }



        [Fact]
        public void Login_Invalid_Shows_Validation()
        {
            Driver.Navigate().GoToUrl(BASE_URL + "Account/Login");
            WaitFor(d => d.FindElements(By.CssSelector("form")).FirstOrDefault());

            SetText("Email", "doesnotexist@example.com");
            SetText("Password", "Wrong!123");
            ClickSubmit();

            var stayedOnLogin = new Uri(Driver.Url).AbsolutePath
                                .IndexOf("/Account/Login", StringComparison.OrdinalIgnoreCase) >= 0;
            Assert.True(stayedOnLogin);

            var src = Driver.PageSource;
            var hasMessage =
                src.IndexOf("invalid", StringComparison.OrdinalIgnoreCase) >= 0 ||
                src.IndexOf("incorrect", StringComparison.OrdinalIgnoreCase) >= 0 ||
                src.IndexOf("не", StringComparison.OrdinalIgnoreCase) >= 0 ||
                Driver.FindElements(By.CssSelector(".validation-summary-errors, .text-danger")).Any();

            Assert.True(hasMessage, "Очекував порака за неуспешна најава.");
        }
    }
}
