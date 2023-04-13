using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static APIConsultaOperadora.Result.ConsultaSituacaoAtualResult;

namespace APIConsultaOperadora.EndPoints;

public static class ConsultaOperadora
{
    public static IWebDriver driver = null;
    public static void MapConsultaOperadora(this WebApplication app)
    {
        app.MapGet("/consultaoperadora", (string numeroTelefone) =>
        {
            var _dados = (ConsultaSituacaoAtual)Processor(numeroTelefone);

            if (!_dados.ProcessOk)
                return Results.NotFound(_dados.MsgError);

            return Results.Ok(_dados);
        }
        ).WithName("GetConsultaOperadora");
    }

    #region Metodos

    public static object Processor(string numeroTelefone)
    {
        ConsultaSituacaoAtual _dados = new ConsultaSituacaoAtual();
        try
        {
            #region Iniciar Driver
            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--start-maximized");
            options.AddArguments("disable-infobars");
            options.AddArguments($"--app={"https://qualoperadora.info/"}");

            List<string> eSwitches = new List<string>
                        {
                            "enable-automation"
                        };

            options.AddExcludedArguments(eSwitches);

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService, options);

            /*Habilitar Espera Implicita*/
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            #endregion

            #region Preencher Telefone  

            var telefone = GetSeleniumElement(TipoElemento.ID, "tel");
            if (telefone == null) return _dados;

            telefone.Click();
            telefone.Clear();
            telefone.SendKeys(numeroTelefone);

            var consultar = GetSeleniumElement(TipoElemento.ID, "bto");
            if (consultar == null) return _dados;

            consultar.Click();

            //Thread.Sleep(1500);

            var resultado = GetSeleniumElement(TipoElemento.CLASS, "resultado");

            if (resultado == null)
            {
                return _dados;
            }

            var portabilidadeElement = GetSeleniumElement(TipoElemento.XPATH, "/html/body/div[1]/div[2]/div[2]/p[1]");
            if (portabilidadeElement == null) return _dados;

            _dados.Portabilidade = portabilidadeElement.Text.Trim().Replace("PORTABILIDADE:\r\n", "");

            if (_dados.Portabilidade.Contains("Ops! O número não foi encontrado."))
            {
                _dados.MsgError = _dados.Portabilidade;
                _dados.Portabilidade = "Não";

                return _dados;
            }

            var existeH2 = GetSeleniumElement(TipoElemento.TAGNAME, "h2");

            if (existeH2 == null)
            {
                var tipoTel = GetSeleniumElement(TipoElemento.CLASS, "img");
                if (tipoTel == null) return _dados;

                _dados.TipoTelefone = tipoTel.Text;

                var operadora = GetSeleniumElement(TipoElemento.XPATH, "/html/body/div[1]/div[2]/div[2]/div/img");
                if (operadora == null) return _dados;

                _dados.Prestadora = operadora.GetAttribute("title");
            }
            else
            {
                var listInfo = existeH2.Text.Split('/');

                _dados.Prestadora = listInfo[0].Trim();
                _dados.TipoTelefone = listInfo[1].Trim();
            }


            var ufElement = GetSeleniumElement(TipoElemento.XPATH, "/html/body/div[1]/div[2]/div[2]/p[2]");
            if (ufElement == null) return _dados;

            _dados.Estado = ufElement.Text.Trim().Replace("ESTADO:\r\n", "");

            var regiaoElement = GetSeleniumElement(TipoElemento.XPATH, "/html/body/div[1]/div[2]/div[2]/p[3]");
            if (regiaoElement == null) return _dados;

            _dados.Cidade = regiaoElement.Text.Trim().Replace("REGIÃO:\r\n", "");
            _dados.ProcessOk = true;

            #endregion
        }
        catch (Exception)
        {
        }
        finally
        {
            driver.Close();
            driver.Quit();
            driver.Dispose();
        }

        return _dados;
    }
    public static IWebElement GetSeleniumElement(TipoElemento tipoSelecionador, string valor)
    {
        IWebElement element = null;
        try
        {
            /*Espera Explicita*/
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            switch (tipoSelecionador)
            {
                case TipoElemento.ID:
                    element = wait.Until(driver => driver.FindElement(By.Id(valor)));

                    element = driver.FindElement(By.Id(valor));
                    break;
                case TipoElemento.XPATH:
                    element = wait.Until(driver => driver.FindElement(By.XPath(valor)));

                    element = driver.FindElement(By.XPath(valor));
                    break;
                case TipoElemento.CLASS:
                    element = wait.Until(driver => driver.FindElement(By.ClassName(valor)));

                    element = driver.FindElement(By.ClassName(valor));
                    break;
                case TipoElemento.NAME:
                    element = wait.Until(driver => driver.FindElement(By.Name(valor)));

                    element = driver.FindElement(By.Name(valor));
                    break;
                case TipoElemento.TAGNAME:
                    var aaaa = driver.FindElements(By.TagName(valor));
                    if (aaaa.Any())
                    {
                        element = wait.Until(driver => driver.FindElement(By.TagName(valor)));

                        element = driver.FindElement(By.TagName(valor));
                    }
                    break;
                default:
                    break;
            }
        }
        catch (Exception)
        {
            return null;
        }
        return element;
    }
    public enum TipoElemento
    {
        ID,
        NAME,
        CLASS,
        XPATH,
        TAGNAME
    }
    #endregion
}
