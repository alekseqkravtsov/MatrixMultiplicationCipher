using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

                //зашифрованное сообщение
                int[] encryptMessage = Encrypt(message);
                printArray(encryptMessage);

            }
        }

        public void printArray(int[] array)
        {
            int i;
            for (i = 0; i < array.Length; i++)
                Console.Write(array[i] + " ");
        }

        private void printMatrix()
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

        private int GetIndex(char character)
        {
            int index = -1;
            string indexString = "";

            for (int rows = 0; rows < 8; rows++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (alphabet[rows, column] == character)
                    {
                        indexString += (rows + 1).ToString() + (column + 1).ToString();
                        index = int.Parse(indexString);
                        break;
                    }
                }

                if (indexString != "")
                    break;

            }

            return index;
        }

        private int[] Encrypt(string message)
        {
            char[] characters = message.ToCharArray();
            int [] lockedMessage = new int [characters.Length];

            int i = 0;
            foreach (var character in characters)
            {
                if (GetIndex(character) == -1)
                    return null;
                else
                {
                    lockedMessage[i] = GetIndex(character);
                    i++;
                }
            }
     
            return lockedMessage;
        }

        static void Main(string[] args)
        {
            Cryptography cryptography = new Cryptography();
            cryptography.startProgram();
        }
    }
}
