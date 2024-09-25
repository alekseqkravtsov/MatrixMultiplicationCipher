using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
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
                if (EncryptPolybius(message) != null)
                {
                    int[,] encryptMessage = EncryptPolybius(message);
                    Console.WriteLine("\nШифрование по таблице алфавита:"); printMatrix(encryptMessage);

                    //инициализируем матрицу ключа
                    int[,] key = initialKeyMatrix(3);
                    Console.WriteLine("\nМатрица ключа:"); printMatrix(key, false);

                    //шифрование перемножением матриц
                    int[,] lockedMessage = Encrypt(message);

                    //дешифрование сообщения
                    string decryptMessage = Decrypt(lockedMessage);
                }
                else
                    Console.WriteLine("Сообщение содержит недопустимые символы для алфавита шифрования.");
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

        private void printMatrix(int[,] matrix, bool onStroke = true)
        {
            int i, j;
            for (i = 0; i < matrix.GetLength(0); i++)
            {
                for (j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                if(!onStroke)
                    Console.WriteLine();
            }

            if(onStroke)
                Console.WriteLine();
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

        private string GetSymbol(int index)
        {
            string symbol = "";
            
            int row = (index / 10) - 1;
            int column = (index % 10) - 1;

            symbol += alphabet[row, column];
            return symbol;
        }

        private int[,] initialKeyMatrix(int n)
        {
            key = new int[n, n];
            Random rnd = new Random();

            for (int i = 0; i < n; i++)
                for (int j = 0;j < n; j++)
                    key[i, j] = rnd.Next(2,10);

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
                        partOfMessage[k, 0] = 84;
                    else
                        partOfMessage[k, 0] = vectorMessage[j, 0];
                }
                
                //умножение матриц
                int[,] result = Multiplication(key, partOfMessage);

                //сливание result в массив, который будет содержать зашифрованное сообщение
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

        private string Decrypt(int[,] lockedMessage)
        {
            string message = "";
            int sections = lockedMessage.GetLength(0) / key.GetLength(0);
            double[,] decryptedParts = new double[key.GetLength(0), 1];

            // Обратное умножение матриц
            for (int i = 0; i < sections; i++)
            {
                int[,] partMessage = new int[key.GetLength(0), 1];
                for (int j = 0; j < key.GetLength(0); j++)
                {
                    partMessage[j, 0] = lockedMessage[i * key.GetLength(0) + j, 0];
                }
               
                //умножение обратной матрицы ключа на зашифрованную часть сообщения
                int[,] result = Multiplication(Inverse(key), partMessage);
                for(int j = 0; j < key.GetLength(0); j++)
                {
                        message += GetSymbol(result[j, 0]);
                }
            }


            Console.WriteLine("\nРасшифрованное сообщение: " + message);
            return message;
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
                        
                        result[i, j] += (matrix[i, k] * partMessage[k, j]);
                    }
                }
            }
            return result;
        }

        private int[,] Multiplication(double[,] matrix, int[,] partMessage)
        {
            
            if (matrix.GetLength(1) != partMessage.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");

            double[,] result = new double[matrix.GetLength(0), partMessage.GetLength(1)];
            int[,] intresult = new int[matrix.GetLength(0), partMessage.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < partMessage.GetLength(1); j++)
                {
                    for (int k = 0; k < partMessage.GetLength(0); k++)
                    {
                        double element = (matrix[i, k] * partMessage[k, j]);
                        result[i, j] += element;
                    }
                    intresult[i, j] = Convert.ToInt32(result[i,j]);
                }
            }

            return intresult;
        }

        private double[,] Inverse(int[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] augmentedMatrix = new double[n, 2 * n];

            // Создаем расширенную матрицу [matrix | I]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, i + n] = 1; // единичная матрица
            }

            // Применяем метод Гаусса
            for (int i = 0; i < n; i++)
            {
                // Находим ведущий элемент
                double pivot = augmentedMatrix[i, i];
                if (Math.Abs(pivot) < 1e-10) // Проверка на ноль
                {
                    throw new InvalidOperationException("Матрица не обратима.");
                }

                // Нормализуем строку
                for (int j = 0; j < 2 * n; j++)
                {
                    augmentedMatrix[i, j] /= pivot;
                }

                // Обнуляем остальные элементы в столбце
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmentedMatrix[k, i];
                        for (int j = 0; j < 2 * n; j++)
                        {
                            augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                        }
                    }
                }
            }

            // Извлекаем обратную матрицу
            double[,] inverseMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    inverseMatrix[i, j] = augmentedMatrix[i, j + n];
                }
            }

            return inverseMatrix;
        }

        static void Main(string[] args)
        {
            Cryptography cryptography = new Cryptography();
            cryptography.startProgram();
        }
    }
}
