using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Prakt4
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private DateTime? _userSelectedBirthday;
        private DateTime _nowDateTime;

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

        public MainWindowViewModel()
        {
            NowDateTime = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    class CalculationOfTheNumberOfYearsPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null) return null;
            string result = (DateTime.Now.Year - ((DateTime)value).Year).ToString();
            switch(result[result.Length - 1])
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

    class CalculationOfTheNumberOfMonthPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            string result = ((DateTime.Now.Year - ((DateTime)value).Year) * 12 + (DateTime.Now.Month - ((DateTime)value).Month)).ToString();
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

    class CalculationOfTheNumberOfDaysPassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            string result = Math.Truncate((DateTime.Now - (DateTime)value).TotalDays).ToString();
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
}
