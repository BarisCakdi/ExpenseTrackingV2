using Microsoft.VisualBasic;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpenseTrackingV2;
class User //kullanıcı kayıt alma
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
class ExpenseTracking // harcama yapan kullanıcı ve eklediği ürünler
{
    public string UserName { get; set; }
    public string CategoryName { get; set; }
    public string ProductName { get; set; }
    public double Amount { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}

internal class Program
{
    static string Quest(string quest)
    {
        Console.Write(quest);
        return Console.ReadLine();
    }
    static List<User> users = new List<User>(); //Kayıt olan kullanıcıların tutulduğu liste
    static User loggedUser = new User(); //giriş yapan kullanıcıyı kim olduğunu tutma yeri
    static List<ExpenseTracking> expenseTrackings = new List<ExpenseTracking>(); //Harcama yapan kullacının yaptığı harcamaların tutulduğu liste
    static ExpenseTracking traking = new ExpenseTracking();

    static void UserAdd() //Yeni kullanıcı oluşturma
    {
        Console.Clear();
        User user = new User();
        user.UserName = Quest("Kullanıcı adı oluşturunuz: ");
        user.Password = Quest("Şifre oluşturunuz: ");
        Console.WriteLine("Kayıt oluşturulmuştur.");
        users.Add(user);
        TxtSaveUsers();
        ReturnMenu();
    }
    static void TxtSaveUsers() //kullanıcı kaydını txt kaydetme
    {
        using StreamWriter writer = new StreamWriter("users.txt");
        foreach (var user in users)
        {
            writer.WriteLine($"{user.UserName} | {user.Password}");
        }
    }
    static void TxtSaveExpenses(User user) //işlem yapan kullanıcının yapıtğı işlemleri txt kaydetme kısmı
    {
        string fileName = $"{user.UserName}_expenses.txt"; //Dosya ismini giriş yapan kullanıcı adı olarak belirliyorum
        using StreamWriter writer = new StreamWriter(fileName);
        //LINQ, C# da veri koleksiyolarını sorgulamak için kullanılan bir araçtır.
        foreach (var expense in expenseTrackings.Where(e => e.UserName == user.UserName))//'Where' bir 'LINQ' methodudur. Burada giriş yapan kullanıcının yaptığı harcamaları kayıt altına almamızı sağlıyor. 
        {
            writer.WriteLine($"{expense.CategoryName} | {expense.ProductName} | {expense.Amount}");
        }
    }
    static void TxtExpenseShow(User user)
    {
        string fileName = $"{user.UserName}_expenses.txt"; //Dosya ismini giriş yapan kullanıcı adı olarak belirliyorum
        using StreamReader reader = new StreamReader(fileName);
        string line;
        while (((line = reader.ReadLine()) != null))
        {
            string[] parts = line.Split('|');
            expenseTrackings.Add(new ExpenseTracking { CategoryName = parts[0].Trim(), ProductName = parts[1].Trim(), Amount = double.Parse(parts[2].Trim()) });
        }
    }
    static void RegisteredUserStorageArea() //kayıtlı kullanıcıyı saklama alanı
    {
        if (File.Exists("users.txt"))
        {
            using StreamReader reader = new StreamReader("users.txt");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split('|');
                users.Add(new User { UserName = parts[0].Trim(), Password = parts[1].Trim() });//Trim = kayıt edilen kullanıcı adı ve şifrede gereksiz boşlukları silmek için kullanıyoruz.
            }
        }
    }
    static void Opening(bool firstOpenin = false)//Girişi bir döngüye alıyorum. Döngü 'True' olana kadar döngüden çıkmıyor.
    {
        Console.Clear();
        if (firstOpenin)
        {
            Console.WriteLine("GİDER TAKİP SİSTEMİ");
            Console.WriteLine("1- Yeni kayıt oluştur");
            Console.WriteLine("2- Giriş yap");
            Console.WriteLine("3- Çıkış");
            char inputchoose = Console.ReadKey().KeyChar;
            switch (inputchoose)
            {
                case '1':
                    UserAdd();
                    break;
                case '2':
                    Start();
                    break;
                case '3':
                    break;
                default:
                    Console.WriteLine("Hatalı seçim!");
                    ReturnMenu();
                    break;
            }
        }
    }
    static void Start() //İşlem menüsüne giriş yapabilmek için bu değişken kullanılıyor.
    {
        while (true)
        {
            Console.Clear();
            Console.Write("Kullanıcı adı giriniz: ");
            var inputName = Console.ReadLine();
            Console.Write("Şifrenizi Giriniz: ");
            var inputPass = Console.ReadLine();
            Console.Clear();
            //LINQ, C# da veri koleksiyolarını sorgulamak için kullanılan bir araçtır.
            //FirsOrDefault bir LINQ operatörüdür. Sorguda ilk eşleleşen öğeyi veya varsayılan değeri döndürür. Hiç bir öğe eşleşmezse 'Null' döner.
            loggedUser = users.FirstOrDefault(u => u.UserName == inputName && u.Password == inputPass);//Kullanılan 'u' değişkeni her öğe için geçiçi bir değişkeni temsil eder.
            if (loggedUser != null)
            {
                Console.WriteLine("Giriş başarılı! Hoş geldin " + loggedUser.UserName);
                AgainExpense();
            }
            else
            {
                Console.WriteLine("Kullanıcı adı veya şifre hatalı. Lütfen tekrar deneyin!");
                ReturnMenu();
            }
            break;
        }
    }
    static void ReturnMenu() //yanlış giriş yapıldığında bu kısım aktif olup giriş ekranına döndürüyor.
    {
        Console.WriteLine("Ana menü için bir tuşa basınız.");
        Console.ReadKey();
        Opening(true);
    }
    static void Menu() //Openin döngüsünden çıktıktan sonra bu değişken çalışmaya başlar
    {
        Console.Clear();
        Console.WriteLine($"Gider takip uygulamasına Hoş geldin {loggedUser.UserName}");
        Console.WriteLine("=============================================================");
        Console.WriteLine("1- Gider ekle");
        Console.WriteLine("2- Geçmiş Tarihli gider oluştur");
        Console.WriteLine("3- Rapor oluştur");
        Console.WriteLine("4- Çıkış yap");
        char inputchoose = Console.ReadKey().KeyChar;
        switch (inputchoose)
        {
            case '1':
                ExpenseAdd();
                break;
            case '2':
                PastDatedExpense();
                break;
            case '3':
                CreateReport();
                break;
            case '4':
                ReturnMenu();
                break;
            default:
                Console.WriteLine("Hatalı seçim!");
                AgainExpense();
                break;
        }
    }
    static void ExpenseAdd() //Gider Oluşturmak için kayıt alanı
    {
        Console.Clear();
        ExpenseTracking expense = new ExpenseTracking();
        expense.UserName = loggedUser.UserName;
        expense.CategoryName = Quest("Katagori giriniz: ");
        expense.ProductName = Quest("Ürün ismi giriniz: ");
        expense.Amount = double.Parse(Quest("Fiyatı giriniz: "));
        expense.Time = DateTime.Now;
        Console.WriteLine("Kayıt oluşturulmuştur.");
        expenseTrackings.Add(expense);
        TxtSaveExpenses(loggedUser);
        AgainExpense();
    }
    static void PastDatedExpense()//Geçmiş tarihe gider oluşturma kısmı
    {
        Console.Clear();
        ExpenseTracking expense = new ExpenseTracking();
        expense.UserName = loggedUser.UserName;
        string userInput = Quest("Tarihi giriniz(31.12): ");
        string[] dateParts = userInput.Split('.');
        // Girişin iki parçaya ayrıldığını ve her parçanın uygun bir tarih bileşeni olduğunu kontrol et
        if (dateParts.Length == 2 && int.TryParse(dateParts[0], out int day) && int.TryParse(dateParts[1], out int month))
        {
            if (day >= 1 && day <= 31 && month >= 1 && month <= 12)
            {
                // Tarih uygun, işlem yapabilirsiniz
                expense.Time = new DateTime(DateTime.Now.Year, month, day);
            }
            else
            {
                Console.WriteLine("Hatalı giriş. Lütfen geçerli bir tarih girin.");
                Console.ReadKey();
                PastDatedExpense();
            }
        }
        else
        {
            Console.WriteLine("Hatalı giriş. Lütfen gün ve ayı belirten iki rakam girin.");
            Console.ReadKey();
            PastDatedExpense();
        }
        expense.CategoryName = Quest("Katagori giriniz: ");
        expense.ProductName = Quest("Ürün ismi giriniz: ");
        expense.Amount = double.Parse(Quest("Fiyatı giriniz: "));
        Console.WriteLine("Kayıt oluşturulmuştur.");
        expenseTrackings.Add(expense);
        TxtSaveExpenses(loggedUser);
        AgainExpense();
    }
    static void AgainExpense()//Gider oluşturduktan sonra tekrar işlem seçmeye yönlendirme
    {
        Console.WriteLine("Devam etmek için bir tuşa basınız");
        Console.ReadKey();
        Menu();
    }
    static void CreateReport()//Yapılan harcamaları listeleme kısmı
    {
        Console.Clear();
        Console.WriteLine("\nTüm kayıtlar");
        Console.WriteLine("=======================================");
        // Ay bazında harcamaları gruplayın
        var expensesByMonth = expenseTrackings
            .Where(e => e.UserName == loggedUser.UserName)
            .GroupBy(e => new { Month = e.Time.Month, Year = e.Time.Year });
        foreach (var monthGroup in expensesByMonth)
        {
            // Ay ve yıl bilgisini alın
            int month = monthGroup.Key.Month;
            int year = monthGroup.Key.Year;
            // Ay bazında toplam harcamayı hesaplayın
            double totalAmountByMonth = monthGroup.Sum(e => e.Amount);
            // Seçilen ayın harcamalarını listele
            Console.WriteLine($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Toplam harcama: {totalAmountByMonth}");
            Console.WriteLine("=======================================");
        }
        Console.WriteLine("\nHangi ayın harcamalarını görmek istiyorsunuz? (Ay ve yıl olarak giriniz, örn. Ocak 2024)");
        string input = Console.ReadLine();
        DateTime selectedDate;
        if (DateTime.TryParseExact(input, "MMMM yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out selectedDate))
        {
            var selectedMonthExpenses = expenseTrackings
                .Where(e => e.UserName == loggedUser.UserName && e.Time.Month == selectedDate.Month && e.Time.Year == selectedDate.Year)
                .ToList();
            if (selectedMonthExpenses.Any())
            {
                Console.Clear();
                Console.WriteLine($"\n{selectedDate:MMMM yyyy} ayının harcamaları");
                Console.WriteLine("=======================================");
                double totalAmount = 0;
                foreach (var expense in selectedMonthExpenses)
                {
                    Console.WriteLine(expense.Time.ToString("dd.MM.yyyy"));
                    Console.WriteLine($"{expense.UserName} - {expense.CategoryName} - {expense.ProductName} - {expense.Amount}");
                    Console.WriteLine("===================================");
                    totalAmount += expense.Amount;
                }
                Console.WriteLine($"Yapılan toplam harcama: {totalAmount}");
            }
            else
            {
                Console.WriteLine("Seçtiğiniz ay için kayıt bulunamadı.");
            }
        }
        else
        {
            Console.WriteLine("Geçersiz tarih formatı. Lütfen ay ve yılı ayın adı ve yıl olarak giriniz (örn. Ocak 2024).");
            Console.ReadKey();
            CreateReport();
        }
        AgainExpense();
    }
    static void Main(string[] args)
    {
        RegisteredUserStorageArea();
        //TxtExpenseShow(loggedUser); ???
        Opening(true);

    }



}