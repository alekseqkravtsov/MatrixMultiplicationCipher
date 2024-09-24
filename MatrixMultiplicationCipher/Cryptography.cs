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

        private int[,] key;

        public void startProgram()
        {
            printMatrix();

            while (true)
            {
                Console.Write("\nВведите сообщение: ");
                string message = Console.ReadLine();

                //зашифрованное сообщение
                int[,] encryptMessage = EncryptPolybius(message);
                Console.WriteLine("\nШифрование по таблице алфавита:"); printMatrix(encryptMessage);

                //инициализируем матрицу ключа
                int[,] key = initialKeyMatrix(3);
                Console.WriteLine("\nМатрица ключа:"); printMatrix(key);

                //шифрование перемножением матриц
                int [,] lockedMessage = Encrypt(message);

            }
        }

        private int getCountSections(int[,] matrix)
        {
            int sections = 1;
            for(int i = 0, j = 0; i < matrix.GetLength(0); i++, j++)
            {
                if (j == 3)
                {
                    sections++;
                    j = 0;
                }
            }
            return sections;
        }

        private void printArray(int[] array)
        {
            int i;
            for (i = 0; i < array.Length; i++)
                Console.Write(array[i] + " ");
            Console.WriteLine();
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

        private void printMatrix(int[,] matrix)
        {
            int i, j;
            for (i = 0; i < matrix.GetLength(0); i++)
            {
                for (j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
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

        private int[,] initialKeyMatrix(int n)
        {
            key = new int[n, n];
            Random rnd = new Random();

            for (int i = 0; i < n; i++)
                for (int j = 0;j < n; j++)
                    key[i, j] = rnd.Next(1,6);

            return key;
        }

        private int[,] EncryptPolybius(string message)
        {
            char[] characters = message.ToCharArray();
            int [,] lockedMessage = new int [characters.Length, 1];

            int i = 0;
            foreach (var character in characters)
            {
                if (GetIndex(character) == -1)
                    return null;
                else
                {
                    lockedMessage[i, 0] = GetIndex(character);
                    i++;
                }
            }

            return lockedMessage;
        }

        private int[,] Encrypt(string message)
        {
            int[,] vectorMessage = EncryptPolybius(message);
            int sections = getCountSections(vectorMessage);
            int[,] partOfMessage = new int[key.GetLength(0), 1];

            int[,] lockedMessage = new int[sections * key.GetLength(0), 1];

            int section = 1, start = 0;
            for (int i = 0; i < sections; i++)
            {
                //разбиение сообщения на части
                for (int j = start, k = 0; j < key.GetLength(0) * section; j++, k++)
                {
                    if (j > vectorMessage.GetLength(0) - 1)
                        partOfMessage[k, 0] = alphabet[7, 3];
                    else
                        partOfMessage[k, 0] = vectorMessage[j, 0];
                }
                
                //умножение матриц
                int[,] result = Multiplication(key, partOfMessage);
                for (int j = start, k = 0; j < section * key.GetLength(0); j++, k++)
                    lockedMessage[j, 0] = result[k, 0];

             
                section++;
                start += key.GetLength(0);
            }

            //вывод зашифрованного сообщения
            Console.WriteLine("\nЗашифрованное сообщение перемножением матриц:");
            printMatrix(lockedMessage);
            return lockedMessage;
            
        }
        
        private int[,] Multiplication(int[,] matrix, int[,] partMessage)
        {
            if (matrix.GetLength(1) != partMessage.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            int[,] result = new int[matrix.GetLength(0), partMessage.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < partMessage.GetLength(1); j++)
                {
                    for (int k = 0; k < partMessage.GetLength(0); k++)
                    {
                        result[i, j] += matrix[i, k] * partMessage[k, j];
                    }
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            Cryptography cryptography = new Cryptography();
            cryptography.startProgram();
        }
    }
}
