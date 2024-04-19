using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Selenuimtests_ambrushkevich;

public class SelenuimTestsForPractice
{
    public ChromeDriver driver;
    public const string FolderName = "Folder223565";

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        driver = new ChromeDriver(options);
        // НЕЯВНЫЕ ОЖИДАНИЯ
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        
        Authorization();
    }
    
    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
    
    public void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        
        // ВВОД ЛОГИНА И ПАРОЛЯ
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("ambrushkevichaa@gmail.com");
        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys("{d#AF<M184ad58m*");
        
        // НАЖАТИЕ НА КНОПКУ ВОЙТИ
        var enter = driver.FindElement(By.ClassName("form-button"));
        enter.Click();

        var pageTitle = driver.FindElement((By.CssSelector("[data-tid='Title']")));
        pageTitle.Text.Should().Be("Новости");
    }
    
    [Test]
    public void BtnCommunityTest()
    {
        // переходим в сообщества
        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();
        
        // проверяем, что на странице есть заголовок "Сообщества" + правильный url
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        communityTitle.Text.Should().Be("Сообщества");
        var url = driver.Url;
        url.Should().Be("https://staff-testing.testkontur.ru/communities");
    }

    [Test]
    public void BtnSecurityTest()
    {
        // клик по иконке профиля
        var dropdownButton = driver.FindElement(By.CssSelector("[data-tid='DropdownButton']"));
        dropdownButton.Click();
        // клик по кнопке "Безопасность"
        var securityButton = driver.FindElement(By.CssSelector("[data-tid='Security'"));
        securityButton.Click();
        
        // проверки: есть заголовок "Безопасность" + правильный url
        var securityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        securityTitle.Text.Should().Be("Безопасность");
        var url = driver.Url;
        url.Should().Be("https://staff-testing.testkontur.ru/security");
    }

    [Test]
    public void GoToFilesPageTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/files");
        
        // проверка заголовка на странице
        var pageTitle = driver.FindElement(By.CssSelector("[title='Файлы']"));
        pageTitle.Text.Should().Be("Файлы");
    }
    
    [Test]
    public void CreateFolderTest()
    {
        GoToFilesPageTest();
        
        // клик по кнопке "Добавить"
        var openActionsBtn = driver.FindElements(By.CssSelector("[data-tid='Actions']"))
            .First(element => element.Text == "Добавить");
        openActionsBtn.Click();
        
        // клик по кнопке "Папку"
        var addFolderButton = driver.FindElement(By.CssSelector("[data-tid='CreateFolder']"));
        addFolderButton.Click();
        
        // проверка заголовка окна
        var windowsTitle = driver.FindElement(By.CssSelector("[data-tid='ModalPageHeader']"));
        windowsTitle.Text.Should().Be("Создать");

        // ввод названия папки
        var folderNameField = driver.FindElement(By.CssSelector("[placeholder='Новая папка']"));
        folderNameField.SendKeys(FolderName);
        
        // нажатие на кнопку "Сохранить"
        var saveBtn = driver.FindElement(By.CssSelector("[data-tid='SaveButton']"));
        saveBtn.Click();
        
        // проврека, что папка с таким названием создалась
        driver.Navigate().Refresh();
        var folder = driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"))
            .First(element => element.Text == FolderName);
        folder.Text.Should().Be(FolderName);
    }

    [Test]
    public void DeleteFolderTest()
    {
        GoToFilesPageTest();
        
        // находим папку
        var folderLine = driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"))
            .First(element => element.Text == FolderName);
        
        // клик по трем точкам справа от папки
        var kebabMenu = folderLine.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']"));
        kebabMenu.Click();
        
        // в списке действий клик по кнопке "Удалить"
        var deleteWindowBtn = driver.FindElement(By.CssSelector("[data-tid='DeleteFile']"));
        deleteWindowBtn.Click();
        
        // клик по кнопке "Удалить"
        var delBtn = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        delBtn.Click();
        
        // проверка, что папки не осталось
        driver.Navigate().Refresh();
        var elements = driver.FindElements(By.CssSelector("[data-tid='ListItemWrapper']"));
        bool folderExists = false;
        foreach (var element in elements)
            if (element.Text == FolderName)
                folderExists = true;
        folderExists.Should().BeFalse($"Папка {FolderName} должна была удалена");
    }
}