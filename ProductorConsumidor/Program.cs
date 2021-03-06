using System;
using System.Threading;

namespace ProductorConsumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            Semaphore producer = new Semaphore(1, 1);
            Semaphore consumer = new Semaphore(0, 1);

            int[] buffer = new int[20];

            Random rand = new Random();
            int producerCounter = 0, consumerCounter = 0;

            void Produce()
            {
                while (true)
                {
                    producer.WaitOne();
                    if (buffer[producerCounter] == 0)
                        Console.WriteLine("El productor esta trabajando...");
                    else
                        Console.WriteLine("El productor no puede producir(buffer lleno)...");

                    int adding = rand.Next(3, 7), toCount = 0;
                    while (toCount < adding)
                    {
                        if (buffer[producerCounter] == 0)
                        {
                            int num = rand.Next(1, 99);
                            buffer[producerCounter] = num;
                            Console.WriteLine("El productor agrego el {0} al buffer en la posicion {1}", num, producerCounter);
                            producerCounter++;
                        }
                        toCount++;
                        if (producerCounter >= 20)
                            producerCounter = 0;
                    }
                    drawBuffer();
                    Console.WriteLine("El productor esta dormido");
                    Thread.Sleep(rand.Next(1, 5) * 400);
                    
                    if ((rand.Next(1, 100) % 2) == 0)
                        consumer.Release();
                    else
                        producer.Release();
                }
            }

            void Consume()
            {
                while (true)
                {
                    consumer.WaitOne();
                    if(buffer[consumerCounter] != 0)
                        Console.WriteLine("El consumidor esta trabajando...");
                    else
                        Console.WriteLine("El consumidor no puede consumir(buffer vacio)...");

                    int taking = rand.Next(3, 7), toCount = 0;
                    while(toCount < taking)
                    {
                        if (buffer[consumerCounter] != 0)
                        {
                            int numTaken = buffer[consumerCounter];
                            buffer[consumerCounter] = 0;
                            Console.WriteLine("El consumidor saco el valor {0} de la posicion {1}", numTaken, consumerCounter);
                            consumerCounter++;
                        }
                        toCount++;
                        if (consumerCounter >= 20)
                            consumerCounter = 0;
                    }
                    drawBuffer();
                    Console.WriteLine("El consumidor esta dormido");
                    Thread.Sleep(rand.Next(1, 5) * 400);

                    if ((rand.Next(1, 100) % 2) == 0)
                        consumer.Release();
                    else
                        producer.Release();
                }
            }

            void drawBuffer()
            {
                string results = "";
                for (int i = 0; i < 20; i++)
                {
                    if (buffer[i] == 0)
                        results += "_|";
                    else
                        results += buffer[i] + "|";
                }
                Console.WriteLine(results);
            }

            void stop()
            {
                ConsoleKeyInfo cki;
                do
                {
                    cki = Console.ReadKey();
                } while (cki.Key != ConsoleKey.Escape);
                Environment.Exit(0);
            }

            Console.WriteLine("Presione ESC para terminar el programa en cualquier momento\n");

            Thread producerThread = new Thread(Produce);
            Thread consumerThread = new Thread(Consume);
            Thread stopCode = new Thread(stop);

            producerThread.Start();
            consumerThread.Start();
            stopCode.Start();

            producerThread.Join();
            consumerThread.Join();
            Console.WriteLine("Presione ESC para salir");

            stopCode.Join();
        }
    }
}
