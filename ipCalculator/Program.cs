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
                int aantNetwerken = Int32.Parse(input);
                int extraBits = toBitsNeeded(aantNetwerken);
                int originalBits = getLengthFromMask(subnetMask_1);
                int newLength = originalBits + extraBits;
                subnetMask_2 = new IPAddress(getMaskFromLength(newLength));
                Console.WriteLine("original length = " + originalBits);
                Console.WriteLine("extra bits = " + extraBits);
                Console.WriteLine("new subnet = " + newLength);
                Console.WriteLine("new subnet = /" + newLength);
                Console.WriteLine("new subnetmask = " + subnetMask_2);

                IPAddress[] netwerken = new IPAddress[aantNetwerken];

                netwerken[0] = ipAdress;
                int octet = 0;
                if (newLength <= 8)
                {
                    octet = 1;
                }
                else if (newLength > 8 && newLength <= 16)
                {
                    octet = 2;
                }
                else if (newLength > 16 && newLength <= 24)
                {
                    octet = 3;
                }
                else if (newLength > 24 && newLength <= 32)
                {
                    octet = 4;
                }

                byte[] bytes = new byte[4];

                Console.WriteLine("octet = " + octet);

                int positie = newLength - (octet - 1) * 8;
                Console.WriteLine("positie = " + positie);
                positie = 9 - positie;
                Console.WriteLine("positie = " + positie);
                int waarde = (int)Math.Pow(2, positie - 1);
                Console.WriteLine("waarde = " +  waarde);

                Console.WriteLine("1: " + ipAdress + "/" + newLength);
                for (int i = 1; i < aantNetwerken; i++)
                {

                    bytes = netwerken[i - 1].GetAddressBytes();

                    if (Convert.ToInt32(bytes[octet -1]) + waarde <= 255)
                    {
                        bytes[octet - 1] = Convert.ToByte(Convert.ToInt32(bytes[octet - 1]) + waarde);
                    }
                    else
                    {
                        bytes[octet - 1] = Convert.ToByte(0);
                        bytes[octet - 2] = Convert.ToByte(Convert.ToInt32(bytes[octet - 2]) + 1);
                    }

                    
                    netwerken[i] = new IPAddress(bytes);
                    Console.WriteLine((i + 1) + ": " + netwerken[i] + "/" + newLength);
                }


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

        public static int toBitsNeeded(int v)
        {
            //int r = 0;

            //while ((v >>= 1) != 0) // unroll for more speed...
            //{
            //    r++;
            //}

            //return r;

            return (int)Math.Log(v, 2) + 1;
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
            //byte[] bytes = subnetMask.GetAddressBytes();

            //BitArray bits = new BitArray(bytes);
            //int index = 0;
            //bool isTrue = true;
            ////for (int i = 0; i < bits.Length; i++)
            ////{
            ////    if (bits.Get(i))
            ////    {
            ////        Console.Write("1");
            ////    } else
            ////    {
            ////        Console.Write("0");
            ////    }

            ////    if ((i % 8 == 7) && (i != 31))
            ////    {
            ////        Console.Write(".");
            ////    }
            ////}
            ////Console.WriteLine();

            //while (isTrue)
            //{
            //    if (bits.Get(index) == true)
            //    {
            //        index++;
            //    } else
            //    {
            //        isTrue = false;
            //    }
            //}
            //return index;

            try
            {
                Byte[] ipbytes = subnetMask.GetAddressBytes();

                uint subnet = 16777216 * Convert.ToUInt32(ipbytes[0]) +
                    65536 * Convert.ToUInt32(ipbytes[1]) + 256 * Convert.ToUInt32(ipbytes[2]) + Convert.ToUInt32(ipbytes[3]);
                uint mask = 0x80000000;
                uint subnetConsecutiveOnes = 0;

                for (int i = 0; i < 32; i++)
                {
                    if (!(mask & subnet).Equals(mask)) break;

                    subnetConsecutiveOnes++;
                    mask = mask >> 1;
                }

                return (int)subnetConsecutiveOnes;
            }
            catch
            {
                return -1;
            }
        }
    }
}
