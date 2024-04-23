using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace Selenuimtests_Ambrushkevich;

public class SelenuimTestsForPractice
{
    public ChromeDriver Driver;
    public WebDriverWait Wait;
    public const string FolderName = "Folder223565";

    public void Authorization()
    {
        Driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        
        // ввод логина и пароля
        var login = Driver.FindElement(By.Id("Username"));
        var password = Driver.FindElement(By.Id("Password"));
        login.SendKeys("ambrushkevichaa@gmail.com");
        password.SendKeys("{d#AF<M184ad58m*");
        
        // нажатие на кнопку войти
        var enter = Driver.FindElement(By.ClassName("form-button"));
        enter.Click();
        
        // ждем, пока в url появится .../news
        Wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
        
        // ждем, пока станет видимым логотип контур-стафа
        Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[alt='Логотип']")));
    }
    
    public void OpenFilesPage()
    {
        Driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/files");
        // ждем, пока в url появится .../files
        Wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/files"));
        
        // ждем заголовок на странице
        Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[title='Файлы']")));
    }
    
    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        Driver = new ChromeDriver(options);
        
        // НЕЯВНЫЕ ОЖИДАНИЯ
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        // ЯВНЫЕ ОЖИДАНИЯ
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
        
        Authorization();
    }
    
    [TearDown]
    public void TearDown()
    {
        Driver.Close();
        Driver.Quit();
    }
    
    [Test]
    public void AuthorizationTest()
    {
        // проверка заголовка
        var pageTitle = Driver.FindElement((By.CssSelector("[data-tid='Title']")));
        pageTitle.Text.Should().Be("Новости");
    }
    
    [Test]
    public void BtnCommunityTest()
    {
        // переходим в сообщества
        var community = Driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        
        // проверяем, что на странице есть заголовок "Сообщества" + правильный url
        var communityTitle = Driver.FindElement(By.CssSelector("[data-tid='Title']"));
        var url = Driver.Url;
        
        using (new AssertionScope())
        {
            communityTitle.Text.Should().Be("Сообщества");
            url.Should().Be("https://staff-testing.testkontur.ru/communities");
        }
    }

    [Test]
    public void BtnSecurityTest()
    {
        // клик по иконке профиля
        var dropdownButton = Driver.FindElement(By.CssSelector("[data-tid='DropdownButton']"));
        dropdownButton.Click();
        // клик по кнопке "Безопасность"
        var securityButton = Driver.FindElement(By.CssSelector("[data-tid='Security'"));
        securityButton.Click();
        
        // проверки: есть заголовок "Безопасность" + правильный url
        var securityTitle = Driver.FindElement(By.CssSelector("[data-tid='Title']"));
        var url = Driver.Url;


        using (new AssertionScope())
        {
            securityTitle.Text.Should().Be("Безопасность");
            url.Should().Be("https://staff-testing.testkontur.ru/security");
        }
    }
    
    [Test]
    public void OpenFilesPageTest()
    {
        OpenFilesPage();
        
        // проверка заголовка
        var pageTitle = Driver.FindElement((By.CssSelector("[data-tid='Title']")));
        pageTitle.Text.Should().Be("Файлы");
    }
    
    [Test]
    public void CreateFolderTest()
    {
        OpenFilesPage();
        
        // клик по кнопке "Добавить"
        var openActionsBtn = Driver.FindElements(By.CssSelector("[data-tid='Actions']"))
            .First(element => element.Text == "Добавить");
        openActionsBtn.Click();
        
        // клик по кнопке "Папку"
        var addFolderButton = Driver.FindElement(By.CssSelector("[data-tid='CreateFolder']"));
        addFolderButton.Click();
        
        // ждем заголовок окна
        Wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ModalPageHeader']")));

        // ввод названия папки
        var folderNameField = Driver.FindElement(By.CssSelector("[placeholder='Новая папка']"));
        folderNameField.SendKeys(FolderName);
        
        // нажатие на кнопку "Сохранить"
        var saveBtn = Driver.FindElement(By.CssSelector("[data-tid='SaveButton']"));
        saveBtn.Click();
        
        // проврека, что папка с таким названием создалась
        Driver.Navigate().Refresh();
        var folder = Driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"))
            .First(element => element.Text == FolderName);
        folder.Text.Should().Be(FolderName);
    }

    [Test]
    public void DeleteFolderTest()
    {
        OpenFilesPage();
        
        // находим папку
        var folderLine = Driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"))
            .First(element => element.Text == FolderName);
        
        // клик по трем точкам справа от папки
        var kebabMenu = folderLine.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        kebabMenu.Click();
        
        // в списке действий клик по кнопке "Удалить"
        var deleteWindowBtn = Driver.FindElement(By.CssSelector("[data-tid='DeleteFile']"));
        deleteWindowBtn.Click();
        
        // клик по кнопке "Удалить"
        var delBtn = Driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        delBtn.Click();
        
        // проверка, что папки не осталось
        Driver.Navigate().Refresh();
        var elements = Driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"));
        bool folderExists = false;
        foreach (var element in elements)
            if (element.Text == FolderName)
                folderExists = true;
        folderExists.Should().BeFalse($"Папка {FolderName} должна была удалена");
    }
}
