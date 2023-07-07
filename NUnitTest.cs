using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace NUnitDataDriven
{
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
        [Parallelizable(ParallelScope.All)]
        public void OpenCategory(string menuOption)
        {
            driver.Navigate().GoToUrl("https://ecommerce-playground.lambdatest.io/");
            driver.FindElement(By.XPath("//a[normalize-space()='Shop by Category']")).Click();
            driver.FindElement(By.XPath($"//span[normalize-space()='{menuOption}']")).Click();
            Assert.That(Equals(driver.Title, menuOption));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}