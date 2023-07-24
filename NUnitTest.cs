using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace NUnitDataDriven
{

    [Parallelizable(ParallelScope.Children)]
    public class NUnitTest
    {
        private static IWebDriver driver;
        public static string gridURL = "@hub.lambdatest.com/wd/hub";
        private static readonly string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME");
        private static readonly string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");

        [SetUp]
        public void Setup()
        {
            ChromeOptions capabilities = new ChromeOptions();
            capabilities.BrowserVersion = "114.0";
            Dictionary<string, object> ltOptions = new Dictionary<string, object>();
            ltOptions.Add("username", LT_USERNAME);
            ltOptions.Add("accessKey", LT_ACCESS_KEY);
            ltOptions.Add("platformName", "Windows 10");
            ltOptions.Add("project", "NUnit Multiple Tests");
            ltOptions.Add("w3c", true);
            ltOptions.Add("plugin", "c#-c#");
            capabilities.AddAdditionalOption("LT:Options", ltOptions);
            driver = new RemoteWebDriver(new Uri($"https://{LT_USERNAME}:{LT_ACCESS_KEY}{gridURL}"), capabilities);
        }

        [Test]
        [TestCase("Components")]
        [TestCase("Cameras")]
        [TestCase("Software")]
        public void OpenCategory(string menuOption)
        {
            driver.Navigate().GoToUrl("https://ecommerce-playground.lambdatest.io/");
            driver.FindElement(By.XPath("//a[normalize-space()='Shop by Category']")).Click();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            string xpath = $"//span[normalize-space()='{menuOption}']";
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            driver.FindElement(By.XPath(xpath)).Click();
            Assert.That(Equals(driver.Title, menuOption));
        }

        [Test]
        [TestCase("andreea", "test")]
        [TestCase("andreea@getnada.com", "")]
        [TestCase("andreea@getnada.com", "xxxx")]
        public void LoginInvalidCredentials(string email, string password)
        {
            driver.Navigate().GoToUrl("https://ecommerce-playground.lambdatest.io/index.php?route=account/login");
            driver.FindElement(By.Name("email")).SendKeys(email);
            driver.FindElement(By.Id("input-password")).SendKeys(password);
            driver.FindElement(By.CssSelector("input[value='Login']")).Click();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            string xpath = "//div[@class='alert alert-danger alert-dismissible']";
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            Assert.That(driver.FindElement(By.XPath(xpath)).Text.Contains("No match for E-Mail Address and/or Password."));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}