using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
//  Посмотреть список пользователей
//	1) Для каждого пользователя хранится следующая информация: 
//	- фамилия
//	- имя
//	- отчество
//	- номер телефон
//	- e - mail
//	- логин
//	- пароль

//	2) Пароль должен храниться и отображаться в форме ХЭШ-кода. 
//	3) Фамилия, имя и отчество записаны с большой буквы. 
//	4) Номер телефона и e-mail должны быть корректны. 
//	5) При вводе информации необходимо проверять корректность ввода полей с помощью регулярных выражений. ФИО должны меняться в соответствии с правилом 3), если телефон записан корректно, то сохранять его в форме +7-(222)-222-22-22
//	6) пароль должен не менее 8 символов, содержать как минимум одну заглавную букву, одну строчную, одну цифру, один специальный символ. Проверка осуществляется с помощью регулярных выражений
//	7) ФИО не должны содержать цифр

//	Добавить пользователя
//	Удалить пользователя
//	Изменить пользователя
//	Сохранить изменения в файл
//	Отправить сообщение на e-mail пользователя
//	Отсортировать по выбранному полю

namespace CommonTask
{
    class Person
    {
        static Person()
        {

            sha256 = SHA256.Create();
            phoneRegex = new Regex(@"^(\+7|8)\d{10}$");
            passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$");
            emailRegex = new Regex(@"^\w+@\w+\.[a-z]{2,4}$");
            nameRegex = new Regex(@"^[А-Я A-Z][a-z а-я]* [А-Я A-Z][a-z а-я]* [А-Я A-Z][a-z а-я]*$");

        }
        static SHA256 sha256;
        static Regex phoneRegex; //REGularEXpression
        static Regex passwordRegex;
        static Regex emailRegex;
        static Regex nameRegex;


        #region Name
        private string name;
        public string Name 
        {   get => name;
            set {
                if (nameRegex.IsMatch(value))
                {
                    name = value;
                }
                else throw new InvalidDataException();
            } 
        }
        #endregion

        #region Phone
        private string phone;
		public string Phone
		{
            get => phone;
			set => phone = value;
		}
        public void SetPhone(string _phone)
        {
            if (phoneRegex.IsMatch(_phone))
            {
                phone = "+7-(";
                for (int i = 1; i < _phone.Length; i++)
                {
                    if (i == 4) phone += ")-";
                    else if (i == 7 || i == 9) phone += "-";
                    phone += _phone[i];
                }
            }
            else throw new InvalidDataException();
        }
        #endregion

        #region Email
        private string email;
		public string Email
		{
			get => email;
			set 
            {
                if (emailRegex.IsMatch(value))
                {
                    email = value;
                }
                else throw new InvalidDataException();
            }
		}
        #endregion

        #region Login
        private string login;
		public string Login
		{
			get => login; 
			set => login = value;
		}
        #endregion

        #region Password
        private string password;
		public string Password
		{
            get => password;
			set => password = value;
		}
        public void SetPassword(string password)
        {
                if (passwordRegex.IsMatch(password))
                {
                    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                    var hash = Convert.ToHexString(sha256.ComputeHash(passwordBytes));
                    password = hash;
                }
                else
                {
                    throw new InvalidDataException();
                }
        }
        #endregion

        public override string ToString()
        {
            return $"Name: {Name}\nPhone: {Phone}\nEmail: {Email}\nLogin: {Login}\nPassword: {Password}";
        }
    }
}
