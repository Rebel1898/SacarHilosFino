using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Sacar_Hilos_Fino;
using SeleniumExtras.WaitHelpers;
using System.Net;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumRPAChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            string Base = Directory.GetCurrentDirectory()+"\\";
            int pagina = 10739;
            string ruta = Base + @"log_Post_from_page_" + pagina +"_"+ DateTime.Now.ToString("yyyyMMdd_hhmm") +".html";
            string ruta_videos = Base + @"log_videos_from_page_" + pagina + "_" + DateTime.Now.ToString("yyyyMMdd_hhmm") + ".html";
            string title = "<li>peperoni</li>";
            string a_element = "<a href=\"peperoni\">peperoni</a> ";
            string main_page = "https://finofilipino.org";
            List<Resultado> listaFinal = new List<Resultado>();

            try
            {
                new WebClient().DownloadFile("https://addons.mozilla.org/firefox/downloads/latest/ublock-origin/addon-11423598-latest.xpi", "ublock_origin.xpi");
            }
            catch
            {
                File.Copy("ublock_origin-1.56.0.xpi", "ublock_origin.xpi");
            
            }

            new DriverManager().SetUpDriver(new FirefoxConfig());
    
            FirefoxDriver driver = new FirefoxDriver();
           
            driver.InstallAddOnFromFile(Base + "\\ublock_origin.xpi");
            WebDriverWait wait30 = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait30.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            wait30.IgnoreExceptionTypes(typeof(NoSuchElementException));
            driver.Navigate().GoToUrl(main_page);
            driver.Manage().Window.Maximize();
            driver.FindElement(By.Id("didomi-notice-disagree-button")).Click();
            driver.FindElement(By.Id("scrollActive")).Click();
            driver.Navigate().GoToUrl(main_page);


            while (driver.FindElements(By.CssSelector(".next.page-numbers")).Count != 0)
            {
                try
                {
                    System.IO.File.AppendAllText(ruta, "<br>pagina " + pagina + "<br>");
                    System.IO.File.AppendAllText(ruta_videos, "<br>pagina " + pagina + "<br>");

                    wait30.Until(ExpectedConditions.ElementExists(By.CssSelector(".next.page-numbers")));
                    wait30.Until(ExpectedConditions.ElementExists(By.CssSelector(".entry-title")));
                    System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> Entradas = driver.FindElements(By.CssSelector(".entry-title"));

                    foreach (IWebElement Entrada in Entradas)
                        listaFinal.Add(new Resultado(Entrada.Text, Entrada.FindElement(By.TagName("a")).GetAttribute("href")));

                    var videos = driver.FindElements(By.TagName("iframe"));
                    wait30.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("iframe")));

                    foreach (IWebElement video in videos)
                    {
                        string marker = video.GetAttribute("src");
                        if (video.GetAttribute("src").Contains("youtube"))
                            listaFinal.Add(new Resultado(video.GetAttribute("title"), video.GetAttribute("src"), "Video"));
                    }

                    var sources = driver.FindElements(By.TagName("source"));
                    foreach (IWebElement video in sources)
                    {
                        string marker = video.GetAttribute("src");
                        if (video.GetAttribute("src").Contains("wordpress"))
                            listaFinal.Add(new Resultado(video.GetAttribute("title"), video.GetAttribute("src"), "Video"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    TryClick(driver, By.Id("amp-didomi-consent-review"));
                }

                try
                {
                    driver.SwitchTo().ParentFrame();
                    wait30.Until(ExpectedConditions.ElementExists(By.CssSelector(".next.page-numbers")));
                    var botonStart = driver.FindElement(By.CssSelector(".next.page-numbers"));
                    botonStart.Click();

                    foreach (Resultado result in listaFinal)
                    {
                        if (result.tipo == "Video")
                        {
                            System.IO.File.AppendAllText(ruta_videos, title.Replace("peperoni", result.titulo));
                            System.IO.File.AppendAllText(ruta_videos, a_element.Replace("peperoni", result.URL));
                        }
                        else
                        {
                            System.IO.File.AppendAllText(ruta, title.Replace("peperoni", result.titulo));
                            System.IO.File.AppendAllText(ruta, a_element.Replace("peperoni", result.URL));
                        }


                    }
                    listaFinal.Clear();
                    wait30.Until(ExpectedConditions.ElementExists(By.CssSelector(".next.page-numbers")));
                    Thread.Sleep(5);
                    pagina++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    TryClick(driver, By.Id("amp-didomi-consent-review"));

                }
            }

            driver.Dispose();
            driver.Quit();
            Console.WriteLine("Execution properly finished");
            Console.WriteLine("Your log files are located at:" + Base);
            Console.WriteLine("Press any key to close this program");
            Console.ReadLine();
        }


        static void TryClick(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by).FindElement(By.TagName(("div"))).Click();
            }
            catch
            {
            }
        }
    }
}