using IPAddressExtensions;
using System.Net;
using System;
using System.Linq;
using System.Collections;
using System.Text;

namespace ipCalculator
{
    class Program
    {

        private int option = 0;
        private StringBuilder breadCrumb = new StringBuilder("You are here: ");

        static void Main(string[] args)
        {
            Console.Title = "FileEncryptor by Dries Stelten";

            Program program = new Program();
        }

        public Program()
        {
            do
            {
                mainMenu();
            } while (option != 4);
        }

        private void mainMenu()
        {
            Console.Clear();
            breadCrumb.Append("Main menu");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();
            Console.WriteLine("Welcome, please select mode:\n");
            Console.WriteLine("1) Network and broadcast address calculator");
            Console.WriteLine("2) Equal subnetting");
            Console.WriteLine("3) VLSM");
            Console.WriteLine("4) Exit\n");
            Console.WriteLine("Enter mode: [1-4]");
            option = getInput(4);

            switch (option)
            {
                case 1:
                    option = 0;
                    Netcalc();
                    break;
                case 2:
                    option = 0;
                    EqualSubnetting();
                    break;
                case 3:
                    option = 0;
                    VLSM();
                    break;
                case 5:
                    break;
                default:
                    Console.WriteLine("error in input, please enter a valid option.");
                    break;
            }
            breadCrumb.Remove(breadCrumb.Length - 9, 9);
        }

        private void Netcalc()
        {
            Console.Clear();
            breadCrumb.Append(" -> Network Calculator");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();

            IPAddress ipAdress;
            IPAddress subnetMask;

            Console.WriteLine("ip plox");
            string input = Console.ReadLine();

            while (input != "c")
            {
                ipAdress = IPAddress.Parse(inputIPAddress(input));
                Console.WriteLine("subnetmask plox");
                input = Console.ReadLine();
                if (input.StartsWith("/"))
                {
                    int netPartLength = Int32.Parse(input.Substring(1, input.Length - 1));
                    subnetMask = new IPAddress(getMaskFromLength(netPartLength));
                    Console.WriteLine("subnetmask = " + subnetMask);
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask));

                }
                else
                {
                    subnetMask = IPAddress.Parse(inputIPAddress(input));
                    Console.WriteLine("slashNotation = /" + getLengthFromMask(subnetMask));
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask) + "\n");
                }
                Console.WriteLine("ip plox");
                input = Console.ReadLine();
            }
            breadCrumb.Remove(breadCrumb.Length - 22, 22);
        }

        private void EqualSubnetting()
        {
            Console.Clear();
            breadCrumb.Append(" -> Equal Subnetting");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();

            IPAddress ipAdress;
            IPAddress subnetMask_1;
            IPAddress subnetMask_2;

            Console.WriteLine("ip plox");
            string input = Console.ReadLine();

            while (input != "c")
            {
                ipAdress = IPAddress.Parse(inputIPAddress(input));
                Console.WriteLine("subnetmask plox");
                input = Console.ReadLine();
                if (input.StartsWith("/"))
                {
                    int netPartLength = Int32.Parse(input.Substring(1, input.Length - 1));
                    subnetMask_1 = new IPAddress(getMaskFromLength(netPartLength));
                    Console.WriteLine("subnetmask = " + subnetMask_1);

                }
                else
                {
                    subnetMask_1 = IPAddress.Parse(inputIPAddress(input));
                }
                Console.WriteLine("How many networks do you want to create?");
                input = Console.ReadLine();
                int extraBits = toBitsNeeded(Int32.Parse(input));

                Console.WriteLine("ip plox");
                input = Console.ReadLine();
            }
            breadCrumb.Remove(breadCrumb.Length - 20, 20);
        }

        private void VLSM()
        {

        }

        private int getInput(int aant)
        {
            bool success = false;
            do
            {
                do
                {
                    try
                    {
                        option = Int32.Parse(Console.ReadLine());
                        success = true;
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        success = false;
                    }
                } while (!success);

                if (!((option > 0) && (option < aant + 1)))
                {
                    Console.WriteLine("please enter a number from 1 to " + aant);
                    success = false;
                }
                else
                {
                    success = true;
                }
            } while (!success);

            return option;
        }

        public static string inputIPAddress(string input)
        {
            bool isValid = false;

            do
            {
                if (!ValidateIPv4(input))
                {
                    isValid = false;
                    Console.WriteLine("Input is not a valid IP address!\nNew ip plox");
                    input = Console.ReadLine();
                }
                else
                {
                    isValid = true;
                }
            } while (!isValid);

            return input;
        }

        public static int toBitsNeeded(int input)
        {
            int extraBits = 0;
            return extraBits;
        }

        public static string inputIPAddress()
        {
            string input = Console.ReadLine();
            return inputIPAddress(input);
        }

        public static bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static byte[] getMaskFromLength(int netPartLength)
        {
            Byte[] binaryMask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                if (i * 8 + 8 <= netPartLength)
                    binaryMask[i] = (byte)255;
                else if (i * 8 > netPartLength)
                    binaryMask[i] = (byte)0;
                else
                {
                    int oneLength = netPartLength - i * 8;
                    string binaryDigit = String.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                    binaryMask[i] = Convert.ToByte(binaryDigit, 2);
                }
            }
            return binaryMask;
        }

        public static int getLengthFromMask(IPAddress subnetMask)
        {
            byte[] bytes = subnetMask.GetAddressBytes();

            BitArray bits = new BitArray(bytes);
            int index = 0;
            bool isTrue = true;
            //for (int i = 0; i < bits.Length; i++)
            //{
            //    if (bits.Get(i))
            //    {
            //        Console.Write("1");
            //    } else
            //    {
            //        Console.Write("0");
            //    }

            //    if ((i % 8 == 7) && (i != 31))
            //    {
            //        Console.Write(".");
            //    }
            //}
            //Console.WriteLine();

            while (isTrue)
            {
                if (bits.Get(index) == true)
                {
                    index++;
                } else
                {
                    isTrue = false;
                }
            }
            return index;
        }
    }
}
