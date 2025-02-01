using System.ComponentModel;
using System.Runtime.CompilerServices;
using TicketAppDesktop.Models;

namespace TicketAppDesktop.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _calculator = new();
        private double _num1;
        private double _num2;
        private double _result;

        public double Num1
        {
            get => _num1;
            set { _num1 = value; OnPropertyChanged(); }
        }

        public double Num2
        {
            get => _num2;
            set { _num2 = value; OnPropertyChanged(); }
        }

        public double Result
        {
            get => _result;
            private set { _result = value; OnPropertyChanged(); }
        }

        public void Add() => Result = _calculator.Add(Num1, Num2);
        public void Subtract() => Result = _calculator.Subtract(Num1, Num2);
        public void Multiply() => Result = _calculator.Multiply(Num1, Num2);
        public void Divide()
        {
            try { Result = _calculator.Divide(Num1, Num2); }
            catch (DivideByZeroException) { Result = double.NaN; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
