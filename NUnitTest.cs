using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

[assembly:Parallelizable(ParallelScope.All)]

namespace NUnitDataDriven
{
    public class NUnitTest
    {
        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        public static string gridURL = "@hub.lambdatest.com/wd/hub";

        public static string testURL_1 = "https://ecommerce-playground.lambdatest.io/";
        public static string testURL_2 = "https://ecommerce-playground.lambdatest.io/index.php?route=account/login";

        private static readonly string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME");
        private static readonly string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY");

        [SetUp]
        public void Setup()
        {
            ChromeOptions capabilities = new ChromeOptions();
            capabilities.BrowserVersion = "latest";
            Dictionary<string, object> ltOptions = new Dictionary<string, object>();
            ltOptions.Add("username", LT_USERNAME);
            ltOptions.Add("accessKey", LT_ACCESS_KEY);
            ltOptions.Add("platformName", "Windows 11");
            ltOptions.Add("project", "NUnit Multiple Tests");
            ltOptions.Add("w3c", true);
            ltOptions.Add("plugin", "c#-c#");
            capabilities.AddAdditionalOption("LT:Options", ltOptions);
            driver.Value = new RemoteWebDriver(new Uri($"https://{LT_USERNAME}:{LT_ACCESS_KEY}{gridURL}"), capabilities);
        }

        [Test]
        [TestCase("Components")]
        [TestCase("Cameras")]
        [TestCase("Software")]
        public void OpenCategory(string menuOption)
        {
            IWebDriver currentDriver = driver.Value;

            currentDriver.Manage().Window.Maximize();
            currentDriver.Navigate().GoToUrl(testURL_1);
            currentDriver.FindElement(By.XPath("//a[normalize-space()='Shop by Category']")).Click();
            WebDriverWait wait = new WebDriverWait(currentDriver, TimeSpan.FromSeconds(5));
            string xpath = $"//span[normalize-space()='{menuOption}']";
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            currentDriver.FindElement(By.XPath(xpath)).Click();
            Assert.That(Equals(currentDriver.Title, menuOption));
        }

        [Test]
        [TestCase("andreea1", "test")]
        [TestCase("andreea2@getnada.com", "")]
        [TestCase("andreea3@getnada.com", "xxxx")]
        public void LoginInvalidCredentials(string email, string password)
        {
            IWebDriver currentDriver = driver.Value;

            currentDriver.Navigate().GoToUrl(testURL_2);
            currentDriver.FindElement(By.Name("email")).SendKeys(email);
            currentDriver.FindElement(By.Id("input-password")).SendKeys(password);
            currentDriver.FindElement(By.CssSelector("input[value='Login']")).Click();
            WebDriverWait wait = new WebDriverWait(currentDriver, TimeSpan.FromSeconds(5));
            string xpath = "//div[@class='alert alert-danger alert-dismissible']";
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
            Assert.That(currentDriver.FindElement(By.XPath(xpath)).Text.Contains("No match for E-Mail Address and/or Password."));
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to Lambdatest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {
                // Terminates the remote webdriver session
                driver.Value.Quit();
            }
        }
    }
}