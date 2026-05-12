using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Globalization;

namespace DatesAndStuff.Web.Tests;

[TestFixture]
public class BlazeDemoTests
{
    private IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }

    [Test]
    public void BlazeDemo_MexicoCityToDublin_ShouldHaveAtLeastThreeFlights()
    {
        // Arrange
        driver.Navigate().GoToUrl("https://blazedemo.com/");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        var departureDropdown = wait.Until(ExpectedConditions.ElementExists(By.XPath("//select[@name='fromPort']")));
        departureDropdown.Click();

        var mexicoCityOption = wait.Until(ExpectedConditions.ElementExists(By.XPath("//select[@name='fromPort']/option[text()='Mexico City']")));
        mexicoCityOption.Click();

        var destinationDropdown = wait.Until(ExpectedConditions.ElementExists(By.XPath("//select[@name='toPort']")));
        destinationDropdown.Click();

        var dublinOption = wait.Until(ExpectedConditions.ElementExists(By.XPath("//select[@name='toPort']/option[text()='Dublin']")));
        dublinOption.Click();

        // Act
        var findFlightsButton = wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@type='submit']")));
        findFlightsButton.Click();

        // Assert
        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//table")));

        var flightRows = driver.FindElements(By.XPath("//table//tbody/tr"));
        flightRows.Count.Should().BeGreaterThanOrEqualTo(3);

        const double maxPrice = 500;

        foreach (var flightRow in flightRows)
        {
            var priceText = flightRow.FindElement(By.XPath("./td[6]")).Text;
            priceText = priceText.Replace("$", "");
            var price = double.Parse(priceText, CultureInfo.InvariantCulture);

            if (price < maxPrice)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var screenshotPath = Path.Combine(desktopPath, "Mexico city-Dublin good price.png");
                File.WriteAllBytes(screenshotPath, screenshot.AsByteArray);
                break;
            }
        }
    }
}