using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace Selenium
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                driver.Navigate().GoToUrl("https://mercantec-ghc.github.io/MAGS-Banko/");

                GeneratePlate("MAGS");
           
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("WebDriver exception: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                driver.Quit();
            }

            void GeneratePlate(string plateId)
            {
                IWebElement inputElement = driver.FindElement(By.Id("tekstboks"));
                inputElement.SendKeys(plateId);

                IWebElement buttonElement = driver.FindElement(By.Id("knap"));
                buttonElement.Click();

                System.Threading.Thread.Sleep(100);

                var tdElements = driver.FindElements(By.TagName("td"));

                List<int> row1 = new List<int>();
                List<int> row2 = new List<int>();
                List<int> row3 = new List<int>();

                int counter = 1;
                foreach (var tdElement in tdElements)
                {
                    string text = tdElement.Text;

                    if (int.TryParse(text, out int number))
                    {
                        if (counter <= 5) { row1.Add(number); }
                        else if (counter <= 10) { row2.Add(number); }
                        else { row3.Add(number); }
                        counter++;
                    }

                }

                Data data = new Data
                {
                    InputText = plateId,
                    Row1 = row1,
                    Row2 = row2,
                    Row3 = row3
                };

                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

                string filePath = "C:\\Users\\MAGS\\Documents\\GitHub\\banko-MAGS-GH\\Selenium\\data.json";
                File.WriteAllText(filePath, jsonString);

                Console.WriteLine($"Data has been written to {filePath}");
            }
        }

        public class Data
        {

            public string InputText { get; set; }
            public List<int> Row1 { get; set; }
            public List<int> Row2 { get; set; }
            public List<int> Row3 { get; set; }
        }
    }

}
