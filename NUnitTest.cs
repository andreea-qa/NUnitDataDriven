using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Support.UI;

namespace NUnitDataDriven
{
    public class NUnitTest
    {
        private static IWebDriver driver;
       
        [SetUp]
        public void Setup()
        {

            driver = new ChromeDriver();
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