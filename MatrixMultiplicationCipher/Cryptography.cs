using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixMultiplicationCipher
{
    internal class Cryptography
    {
        private char[,] alphabet = new char[8, 9]
        {
                { 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З'},
                { 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р'},
                { 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ'},
                { 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', 'а', 'б', 'в'},
                { 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к'},
                { 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у'},
                { 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь'},
                { 'э', 'ю', 'я', ' ', '.', ':', '!', '?', ','},
        };

        public void startProgram()
        {
            printMatrix();

            while (true)
            {
                Console.Write("\nВведите сообщение: ");
                string message = Console.ReadLine();
            }
        }


        public void printMatrix()
        {
            Console.WriteLine("Матрица алфавита:\n");

            int i, j;
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    Console.Write(alphabet[i, j] + " ");
                }
                Console.WriteLine("");
            }
        }


        static void Main(string[] args)
        {
            Cryptography cryptography = new Cryptography();
            cryptography.startProgram();
        }
    }
}
