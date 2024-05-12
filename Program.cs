using CommonTask;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;



string[] actions = new string[] // Список действий
{
    "---------------------------------------------------",
    "a -- Посмотреть список пользователей",
    "b -- Добавить пользователя",
    "c -- Удалить пользователя",
    "d -- Изменить пользователя",
    "e -- Сохранить изменения в файл",
    "f -- Отправить сообщение на e-mail пользователя",
    "g -- Отсортировать по выбранному полю",
    "q -- Выход",
    "---------------------------------------------------"
};
string path = "persons.json";

string name; // имя текущего пользователя для взаимодействия
string login; // логин текущего пользователя для взаимодействия
string phone; // телефон текущего пользователя для взаимодействия
string password;
Person person; // текущий пользователь
ConsoleKey inputKey; 


if (!File.Exists(path)) File.Create(path);
var personsJson = File.ReadAllText(path);


List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(personsJson) ?? new List<Person>();
do
{
    
    Console.WriteLine($"выберете действие:\n{string.Join('\n', actions)}");
    inputKey = Console.ReadKey().Key;
    
    switch (inputKey)
    {
        // Посмотреть список пользователей
        case ConsoleKey.A:
            Console.Clear();
            if (persons.Count == 0) Console.WriteLine("Вы не добавили не одного пользователя");
            Console.WriteLine("----Пользователи----");
            foreach (var _person in persons) Console.WriteLine(_person + "\n");
            Console.WriteLine("--------------------");
            break;


        // Добавить пользователя
        case ConsoleKey.B:
            Console.Clear();
            var newPerson = new Person();
            try
            {
                Console.WriteLine("введите ФИО (ФИО должны быть с большой буквы и не содержать цифры): "); newPerson.Name = Console.ReadLine();
                Console.WriteLine("введите номер телефона: "); newPerson.SetPhone(Console.ReadLine());
                Console.WriteLine("введите почту: "); newPerson.Email = Console.ReadLine();
                Console.WriteLine("введите логин: "); newPerson.Login = Console.ReadLine();
                Console.WriteLine("введите пароль (пароль должен не менее 8 символов, содержать как минимум одну заглавную букву, одну строчную, одну цифру, один специальный символ): "); newPerson.SetPassword(Console.ReadLine());

                persons.Add(newPerson);
            }
            catch (InvalidDataException e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                Console.WriteLine("----Нажмите любую кнопку чтобы начать заново----");
                Console.ReadKey();
                goto case ConsoleKey.B;
            }
            break;


        // Удалить пользователя
        case ConsoleKey.C:
            Console.Clear();
            person = GetPersonByOptions("удалить пользователя");
            if (person is not null)
            {
                if (VerificatePersonPassword(person.Password))
                {
                    persons.Remove(person);
                    Console.WriteLine("----Пользователь удален----");
                }
                else Console.WriteLine("----Пароль не верный----");
            }
            else Console.WriteLine("----Пользователь не найден----");
            break;

        // Изменить пользователя
        case ConsoleKey.D:
            Console.Clear();
            person = GetPersonByOptions("изменить пользователя");
            if (person is not null)
            {
                if (VerificatePersonPassword(person.Password))
                {
                    try
                    {
                        Console.WriteLine("введите ФИО (ФИО должны быть с большой буквы и не содержать цифры): "); person.Name = Console.ReadLine();
                        Console.WriteLine("введите номер телефона: "); person.SetPhone(Console.ReadLine());
                        Console.WriteLine("введите почту: "); person.Email = Console.ReadLine();
                        Console.WriteLine("введите логин: "); person.Login = Console.ReadLine();
                        Console.WriteLine("введите пароль (пароль должен не менее 8 символов, содержать как минимум одну заглавную букву, одну строчную, одну цифру, один специальный символ): "); person.SetPassword(Console.ReadLine());
                        Console.WriteLine("----Данные успешно изменены----");
                    }
                    catch (InvalidDataException e)
                    {
                        Console.WriteLine("ERROR: " + e.Message);
                    }
                }
                else Console.WriteLine("----Пароль не верный----");
            }
            else Console.WriteLine("----Пользователь не найден----");
            break;
        // Сохранить изменения в файл
        case ConsoleKey.E:
            Console.Clear();
            SaveChanges(persons);
            break;
        // Отправить сообщение на e-mail пользователя
        case ConsoleKey.F:
            Console.Clear();
            person = GetPersonByOptions("отправить сообщение на почту");
            if (person is not null)
            {
                Console.WriteLine("введите сообщение для отправки:");
                var message = Console.ReadLine();
                SendMessageToEmail(message, person.Email);
            }
            else Console.WriteLine("----Пользователь не найден----");
            break;
        // Отсортировать по выбранному полю
        case ConsoleKey.G:
            Console.Clear();
            var properties = typeof(Person).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                Console.WriteLine($"{i+1} -- {properties[i].Name}");
            List<Person> sortedPersons = new();
            Console.WriteLine("выберите поле для сортировки:");
            if (int.TryParse(Console.ReadLine(), out int number) )
            {
                switch (number)
                {
                    case 0:
                        sortedPersons = persons.OrderBy(person => person.Name).ToList();
                        break;
                    case 1:
                        sortedPersons = persons.OrderBy(person => person.Phone).ToList();
                        break;
                    case 2:
                        sortedPersons = persons.OrderBy(person  => person.Email).ToList();
                        break;
                    case 3:
                        sortedPersons = persons.OrderBy(person => person.Login).ToList();
                        break;
                    case 4:
                        sortedPersons = persons.OrderBy(person => person.Password).ToList();
                        break;

                }
                Console.WriteLine($"----Отсортированные по {properties[number-1].Name} пользователи----");
                sortedPersons.ForEach(_person => Console.WriteLine(_person + "\n"));
                Console.WriteLine("------------------------------------");
            }
            else Console.WriteLine("ERROR: некорректно введен номер свойств");
            break;
    }

} while (inputKey != ConsoleKey.Q);

Person? GetPersonByOptions(string forWhat)
{
    int option;
    Console.WriteLine("1 -- ФИО\n2 -- Логин\n3 -- Телефон");
    Console.WriteLine($"выберите свойство, по которому хотите {forWhat}:");
    option = Convert.ToInt32(Console.ReadLine());
    switch (option)
    {
        case 1:
            PrintExistNames();
            Console.WriteLine("введите имя:");
            name = Console.ReadLine();
            login = phone = string.Empty;
            break;
        case 2:
            PrintExistLogins();
            Console.WriteLine("введите логин:");
            login = Console.ReadLine();
            name = phone = string.Empty;
            break;
        case 3:
            PrintExistPhones();
            Console.WriteLine("введите телефон:");
            phone = Console.ReadLine();
            login = name = string.Empty;
            break;
        default:
            name = login = phone = string.Empty;
            break;
    }
    return persons.Find(person => person.Name.Contains(name) || person.Phone.Contains(phone) || person.Login.Contains(login));
}
bool VerificatePersonPassword(string correctHash)
{
    Console.WriteLine("введите пароль данного пользователя");
    password = Console.ReadLine();
    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
    var hash = Convert.ToHexString(SHA256.Create().ComputeHash(passwordBytes));
    if (hash  == correctHash) return true;
    return false;
}
void PrintExistNames()
{
    var names = from person in persons select person.Name;
    Console.WriteLine("----Доступные имена пользователей----");
    foreach (var name in names)
        Console.WriteLine(name);
    Console.WriteLine("-------------------------------------");
}
void PrintExistLogins()
{
    var logins = from person in persons select person.Login;
    Console.WriteLine("----Доступные имена пользователей----");
    foreach (var login in logins)
        Console.WriteLine(login);
    Console.WriteLine("-------------------------------------");
}
void PrintExistPhones()
{
    var phones = from person in persons select person.Phone;
    Console.WriteLine("----Доступные имена пользователей----");
    foreach (var phone in phones)
        Console.WriteLine(phone);
    Console.WriteLine("-------------------------------------");
}
void SaveChanges(List<Person> persons)
{   
    var serializePersons = JsonConvert.SerializeObject(persons, Formatting.Indented);
    File.WriteAllText(path, serializePersons);
    Console.WriteLine("----Изменения сохранены в файл----");
}
void SendMessageToEmail(string message, string recipient)
{
    using SmtpClient smtpClient = new SmtpClient("smtp.yandex.ru", 587);
    smtpClient.EnableSsl = true;
    smtpClient.Credentials = new NetworkCredential("bogdanevseyv@yandex.ru", "smdpvxfzbziwwpcb");
    MailMessage mailMessage = new MailMessage();
    mailMessage.From = new MailAddress("bogdanevseyv@yandex.ru");
    mailMessage.To.Add(recipient);
    mailMessage.Subject = "Spam from KubGU";
    mailMessage.Body = message;


    try
    {
        smtpClient.Send(mailMessage);
        Console.WriteLine("----Письмо успешно отправлено----");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}