using System;
using System.Collections.Generic;

namespace MiniProject
{
    internal class QuickFitAllocator
    {
        private Dictionary<int, List<int>> freeLists;
        private List<int?> memoryPool;

        public QuickFitAllocator(List<int> blockSizes)
        {
            freeLists = new Dictionary<int, List<int>>();
            memoryPool = new List<int?>();
            foreach (var size in blockSizes)
            {
                freeLists[size] = new List<int>();
                int address = memoryPool.Count;
                memoryPool.Add(null);
                freeLists[size].Add(address);
            }
        }

        public void Allocate(int size)
        {
            foreach (var blockSize in new SortedSet<int>(freeLists.Keys))
            {
                if (blockSize == size && freeLists[blockSize].Count > 0)
                {
                    int address = freeLists[blockSize][0];
                    freeLists[blockSize].RemoveAt(0);
                    memoryPool[address] = size; // Mark the block as allocated.
                    Console.WriteLine($"Allocated block of size {size} at address {address}.");
                    return;
                }
            }

            int newAddress = memoryPool.Count;
            memoryPool.Add(size); // Mark new block as allocated.
            Console.WriteLine($"Allocated block of size {size} at new address {newAddress}.");
        }

        public void Free(int address)
        {
            if (address >= 0 && address < memoryPool.Count && memoryPool[address] != null)
            {
                int size = memoryPool[address].Value;
                memoryPool[address] = null;

                if (freeLists.ContainsKey(size))
                {
                    freeLists[size].Add(address);
                    Console.WriteLine($"Freed block of size {size} at address {address}.");
                }
                else
                {
                    Console.WriteLine($"Freed block of size {size} at address {address}, but no suitable quick fit free list.");
                }
            }
            else
            {
                Console.WriteLine($"Invalid address {address} for free operation.");
            }
        }

        public bool IsBlockFree(int address)
        {
            return address >= 0 && address < memoryPool.Count && memoryPool[address] == null;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter block sizes separated by spaces:");
            var blockSizes = Array.ConvertAll(Console.ReadLine()?.Split(' ') ?? Array.Empty<string>(), int.Parse);

            var allocator = new QuickFitAllocator(new List<int>(blockSizes));

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Allocate Memory");
                Console.WriteLine("2. Free Memory");
                Console.WriteLine("3. Check if Block is Free");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter size to allocate: ");
                        if (int.TryParse(Console.ReadLine(), out int size))
                        {
                            allocator.Allocate(size);
                        }
                        else
                        {
                            Console.WriteLine("Invalid size input.");
                        }
                        break;
                    case 2:
                        Console.Write("Enter address to free: ");
                        if (int.TryParse(Console.ReadLine(), out int address))
                        {
                            allocator.Free(address);
                        }
                        else
                        {
                            Console.WriteLine("Invalid address input.");
                        }
                        break;
                    case 3:
                        Console.Write("Enter address to check: ");
                        if (int.TryParse(Console.ReadLine(), out int checkAddress))
                        {
                            bool isFree = allocator.IsBlockFree(checkAddress);
                            Console.WriteLine($"Block at address {checkAddress} is {(isFree ? "free" : "not free")}.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid address input.");
                        }
                        break;
                    case 4:
                        Console.WriteLine("Exiting program.");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid menu item.");
                        break;
                }
            }
        }
    }
}