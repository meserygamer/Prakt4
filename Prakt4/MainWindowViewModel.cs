using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using RealizePythonList;

namespace Prakt4
{
    /// <summary>
    /// ViewModel главного окна
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Хранит дату выбранную пользователем
        /// </summary>
        private DateTime? _userSelectedBirthday;
        /// <summary>
        /// Хранит сегодняшнюю дату (Нужен для ограничения календаря)
        /// </summary>
        private DateTime _nowDateTime;
        /// <summary>
        /// Хранит номер выбранного календаря
        /// </summary>
        private int _idCurrentCalendar;

        /// <summary>
        /// Свойство для получения выбранной пользователем даты
        /// </summary>
        public DateTime? UserSelectedBirthday
        {
            get => _userSelectedBirthday;
            set
            {
                _userSelectedBirthday = value;
                OnPropertyChanged("UserSelectedBirthday");
                OnPropertyChanged("NumberOfBirthdaysCelebratedOnTheBirthday");
                OnPropertyChanged("NumberOfLeapYearsExperienced");
            }
        }

        /// <summary>
        /// Свойство для получения текущей даты
        /// </summary>
        public DateTime NowDateTime
        {
            get { 
                return _nowDateTime;
            }
            set
            {
                _nowDateTime = value;
                OnPropertyChanged("NowDateTime");
            }
        }

        /// <summary>
        /// Свойство для получения выбранного календаря
        /// </summary>
        public int IdCurrentCalendar
        {
            get=> _idCurrentCalendar;
            set
            {
                _idCurrentCalendar = value;
                OnPropertyChanged("IdCurrentCalendar");
            }
        }

        /// <summary>
        /// Хранит количество дней рождений отпразднованных в день недели рождения человека
        /// </summary>
        public int NumberOfBirthdaysCelebratedOnTheBirthday
        {
            get
            {
                if(_userSelectedBirthday is null) return 0;
                int counter = 0;
                for(DateTime Birtday = (DateTime)_userSelectedBirthday; Birtday < DateTime.Now; Birtday = Birtday.AddYears(1))
                {
                    if(Birtday.DayOfWeek == ((DateTime)_userSelectedBirthday).DayOfWeek)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        /// <summary>
        /// Хранит количество прошедших весокосных лет с рождения человека
        /// </summary>
        public string NumberOfLeapYearsExperienced
        {
            get
            {
                if (_userSelectedBirthday is null) return null;
                string LeapYears = "";
                int counter = 0;
                for (DateTime Birtday = (DateTime)_userSelectedBirthday; Birtday < DateTime.Now;)
                {
                    if (DateTime.IsLeapYear(Birtday.Year))
                    {
                        counter += 1;
                        LeapYears += Birtday.Year.ToString() + ",";
                        Birtday = Birtday.AddYears(4);
                    }
                    else Birtday = Birtday.AddYears(1);
                }
                if(LeapYears.Length > 0) LeapYears = LeapYears.Remove(LeapYears.Length - 1, 1);
                return counter.ToString() + " (" + LeapYears + ")";
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindowViewModel()
        {
            NowDateTime = DateTime.Now;
        }

        #region реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }

    /// <summary>
    /// Конвертер для получения количества прошедших лет
    /// </summary>
    class CalculationOfTheNumberOfYearsPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null) return null;
            string result;
            if (((DateTime)value).DayOfYear <= DateTime.Today.DayOfYear) result = (DateTime.Today.Year - ((DateTime)value).Year).ToString();
            else result = (DateTime.Today.Year - ((DateTime)value).Year - 1).ToString();
            switch (result[result.Length - 1])
            {
                case '1':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " лет";
                        break;
                    }
                    result += " год";
                    break;
                case '2':
                case '3':
                case '4':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " лет";
                        break;
                    }
                    result += " года";
                    break;
                default:
                    result += " лет";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Конвертер для получения количества прошедших месяцев
    /// </summary>
    class CalculationOfTheNumberOfMonthPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            int Age;
            if (((DateTime)value).DayOfYear <= DateTime.Today.DayOfYear) Age = (DateTime.Today.Year - ((DateTime)value).Year);
            else Age = (DateTime.Today.Year - ((DateTime)value).Year - 1);
            string result;
            if (DateTime.Today.Day - ((DateTime)value).Day < 0) result = (DateTime.Today.Month - ((DateTime)value).AddYears(Age).Month - 1 + 
                    (DateTime.Today.Year - ((DateTime)value).AddYears(Age).Year) * 12).ToString();
            else result = (DateTime.Today.Month - ((DateTime)value).AddYears(Age).Month +
                    (DateTime.Today.Year - ((DateTime)value).AddYears(Age).Year) * 12).ToString();
            switch (result[result.Length - 1])
            {
                case '1':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " месяцев";
                        break;
                    }
                    result += " месяц";
                    break;
                case '2':
                case '3':
                case '4':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " месяцев";
                        break;
                    }
                    result += " месяца";
                    break;
                default:
                    result += " месяцев";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Конвертер для получения количества прошедших дней
    /// </summary>
    class CalculationOfTheNumberOfDaysPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            int Age;
            if (((DateTime)value).DayOfYear <= DateTime.Today.DayOfYear) Age = (DateTime.Today.Year - ((DateTime)value).Year);
            else Age = (DateTime.Today.Year - ((DateTime)value).Year - 1);
            int month;
            if (DateTime.Today.Day - ((DateTime)value).Day < 0) month = (DateTime.Today.Month - ((DateTime)value).AddYears(Age).Month - 1 +
                    (DateTime.Today.Year - ((DateTime)value).AddYears(Age).Year) * 12);
            else month = (DateTime.Today.Month - ((DateTime)value).AddYears(Age).Month +
                    (DateTime.Today.Year - ((DateTime)value).AddYears(Age).Year) * 12);
            string result = (DateTime.Today - ((DateTime)value).AddYears(Age).AddMonths(month)).TotalDays.ToString();
            switch (result[result.Length - 1])
            {
                case '1':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " дней";
                        break;
                    }
                    result += " день";
                    break;
                case '2':
                case '3':
                case '4':
                    if (Regex.IsMatch(result, @"^.*(11|12|13|14)$"))
                    {
                        result += " дней";
                        break;
                    }
                    result += " дня";
                    break;
                default:
                    result += " дней";
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Конвертер для получения дня недели в который родился человек
    /// </summary>
    class CalculateDayOfBirthDay : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null) return null;
            switch(((DateTime)value).DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Вы родились в воскресенье";
                case DayOfWeek.Monday:
                    return "Вы родились в понедельник";
                case DayOfWeek.Tuesday:
                    return "Вы родились во вторник";
                case DayOfWeek.Wednesday:
                    return "Вы родились в среду";
                case DayOfWeek.Thursday:
                    return "Вы родились в четверг";
                case DayOfWeek.Friday:
                    return "Вы родились в пятницу";
                case DayOfWeek.Saturday:
                    return "Вы родились в субботу";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class CalculatePokrovitOfBirthDay : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is null) return null;
            if ((int)value[1] == 0) return null;
            switch((int)value[1])
            {
                case 1:
                    PythonList<string> Animals = new PythonList<string>("Крыса",
                    "Бык", "Тигр", "Кролик", "Дракон", "Змея", "Лошадь", "Овца",
                    "Обезьяна", "Петух", "Собака", "Свинья");
                    PythonList<string> Colors = new PythonList<string>("Инь белый",
                    "Янь белый", "Инь синий", "Янь синий", "Инь зеленый", "Янь зеленый", "Инь красный", "Янь красный",
                    "Инь желтый", "Янь желтый");
                    int Change = ((DateTime)value[0]).Year - 2020;
                    return $"По календарю вы {Colors[Change]} {Animals[Change]}";
                case 2:
                    if ((((DateTime)value[0]).Month == 12 && ((DateTime)value[0]).Day >= 24)
                        || (((DateTime)value[0]).Month == 1 && ((DateTime)value[0]).Day <= 30))
                        return "Ваш покровитель - Мороз";
                    else if ((((DateTime)value[0]).Month == 1 && ((DateTime)value[0]).Day >= 31)
                        || (((DateTime)value[0]).Month == 2 && ((DateTime)value[0]).Day <= 28))
                        return "Ваш покровитель - Велес";
                    else if (((DateTime)value[0]).Month == 3)
                        return "Ваша покровительница - Макошь";
                    else if (((DateTime)value[0]).Month == 4)
                        return "Ваша покровительница - Жива";
                    else if (((DateTime)value[0]).Month == 5 && ((DateTime)value[0]).Day >= 1
                        && ((DateTime)value[0]).Day <= 14)
                        return "Ваш покровитель - Ярило";
                    else if ((((DateTime)value[0]).Month == 5 && ((DateTime)value[0]).Day >= 15)
                        || (((DateTime)value[0]).Month == 6 && ((DateTime)value[0]).Day <= 2)) // 2 июня занято обоими богами, ошибка, время даты косторомы с 3 по 12, а не со 2 по 12
                        return "Ваша покровительница - Леля";
                    else if (((DateTime)value[0]).Month == 6 && ((DateTime)value[0]).Day >= 3
                        && ((DateTime)value[0]).Day <= 12)
                        return "Ваша покровительница - Кострома";
                    else if (((DateTime)value[0]).Month == 6 && ((DateTime)value[0]).Day == 24)
                        return "Ваш покровитель - Иван Купала";
                    else if ((((DateTime)value[0]).Month == 6 && ((DateTime)value[0]).Day >= 13)
                        || (((DateTime)value[0]).Month == 7 && ((DateTime)value[0]).Day <= 6))
                        return "Ваша покровительница - Додола";
                    else if (((DateTime)value[0]).Month == 7 && ((DateTime)value[0]).Day >= 7
                        && ((DateTime)value[0]).Day <= 31)
                        return "Ваша покровительница - Лада";
                    else if (((DateTime)value[0]).Month == 8 && ((DateTime)value[0]).Day >= 1
                        && ((DateTime)value[0]).Day <= 28)
                        return "Ваш покровитель - Перун";
                    else if ((((DateTime)value[0]).Month == 8 && ((DateTime)value[0]).Day >= 29)
                        || (((DateTime)value[0]).Month == 9 && ((DateTime)value[0]).Day <= 13))
                        return "Ваша покровительница - Сева";
                    else if (((DateTime)value[0]).Month == 9 && ((DateTime)value[0]).Day >= 14
                        && ((DateTime)value[0]).Day <= 27)
                        return "Ваш покровитель - Рожаница";
                    else if ((((DateTime)value[0]).Month == 9 && ((DateTime)value[0]).Day >= 28)
                        || (((DateTime)value[0]).Month == 10 && ((DateTime)value[0]).Day <= 15))
                        return "Ваш покровитель - Сварожич";
                    else if ((((DateTime)value[0]).Month == 10 && ((DateTime)value[0]).Day >= 16)
                        || (((DateTime)value[0]).Month == 11 && ((DateTime)value[0]).Day <= 8))
                        return "Ваша покровительница - Морана (Морена)";
                    else if (((DateTime)value[0]).Month == 11 && ((DateTime)value[0]).Day >= 9
                        && ((DateTime)value[0]).Day <= 28)
                        return "Ваша покровительница - Зима";
                    else if ((((DateTime)value[0]).Month == 11 && ((DateTime)value[0]).Day >= 29)
                        || (((DateTime)value[0]).Month == 12 && ((DateTime)value[0]).Day <= 23))
                        return "Ваш покровитель - Корочун";
                    else return "Не удалось определить (29 февраля не покрывается не одним богом в используемой классификации, однако в других классификациях оно закреплено за Родом)";
                default: 
                    return "Если вы это видите, то что-то сломалось";
            }
        }

        public object[] ConvertBack(
        object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
