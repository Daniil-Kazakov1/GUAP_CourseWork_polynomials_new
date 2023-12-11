using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace courseWork_polinom
{
    class Polynomial
    {
        public double[] coefficients; // Массив для хранения коэффициентов многочлена
        private int agree;
        // Конструктор класса для создания объекта Polynomial с переданными коэффициентами
        public Polynomial(double[] coeffs, int agr)
        {
            coefficients = new double[coeffs.Length]; // Создание массива с размером, равным количеству переданных коэффициентов
            Array.Copy(coeffs, coefficients, coeffs.Length); // Копирование переданных коэффициентов в массив coefficients
            agree = agr;
        }
       
    }

}