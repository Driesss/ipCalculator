using IPAddressExtensions;
using System.Net;
using System;
using System.Linq;
using System.Collections;

namespace ipCalculator
{
    class Program
    {

        static void Main(string[] args)
        {
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
                    int netPartLength = Int32.Parse(input.Substring(1, input.Length-1));
                    subnetMask = new IPAddress(getMaskFromLength(netPartLength));
                    Console.WriteLine("subnetmask = " + subnetMask);
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask));

                } else
                {
                    subnetMask = IPAddress.Parse(inputIPAddress(input));
                    Console.WriteLine("slashNotation = /" + getLengthFromMask(subnetMask));
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask) + "\n");
                }
                Console.WriteLine("ip plox");
                input = Console.ReadLine();
            }
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
