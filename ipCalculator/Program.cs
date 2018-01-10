using IPAddressExtensions;
using System.Net;
using System;
using System.Linq;
using System.Collections;
using System.Text;
using System.IO;

namespace ipCalculator
{
    class Program
    {

        private int option = 0; //declare input option var
        private StringBuilder breadCrumb = new StringBuilder("You are here: "); //create stringbuilder for breadcrumb


        static void Main(string[] args)
        {
            Console.Title = "IPCalculator by Dries Stelten"; //set console title

            Program program = new Program();
        }

        public Program()
        {
            do //do main program loop until input option is 4
            {
                mainMenu(); //draw main menu
            } while (option != 4);
        }
        
        /// <summary>
        /// Draws main menu
        /// </summary>
        private void mainMenu()
        {
            //clear console and show menu
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
            //get input
            option = getInput(4);

            //select option
            switch (option)
            {
                //reset option and execute selected item
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
                case 4:
                    break;
                default:
                    //show error in invalid input (should be impossible)
                    Console.WriteLine("error in input, please enter a valid option.");
                    break;
            }
        }

        /// <summary>
        /// show network calculator
        /// </summary>
        private void Netcalc()
        {
            //clear console and write breadcrumb
            Console.Clear();
            breadCrumb.Append(" -> Network Calculator");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();

            //declare var for ip and subnet mask
            IPAddress ipAdress;
            IPAddress subnetMask;

            //ask for ip and get input
            Console.WriteLine("ip plox");
            string input = Console.ReadLine();

            //keep looping until user exits by giving "c" as input
            while (input != "c")
            {
                //parse ip
                ipAdress = IPAddress.Parse(inputIPAddress(input));
                //ask for subnet and get input
                Console.WriteLine("subnetmask plox");
                input = Console.ReadLine();
                //check if mask or cidr is given
                if (input.StartsWith("/"))
                {
                    //get subnetmask from cidr
                    int netPartLength = Int32.Parse(input.Substring(1, input.Length - 1));
                    subnetMask = new IPAddress(getMaskFromLength(netPartLength));
                    //print results
                    Console.WriteLine("subnetmask = " + subnetMask);
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask));

                }
                else
                {
                    //get cidr from subnetmask
                    subnetMask = IPAddress.Parse(inputIPAddress(input));
                    //print results
                    Console.WriteLine("slashNotation = /" + getLengthFromMask(subnetMask));
                    Console.WriteLine("networkAddress = " + ipAdress.GetNetworkAddress(subnetMask));
                    Console.WriteLine("broadcastAddress = " + ipAdress.GetBroadcastAddress(subnetMask) + "\n");
                }

                //get input and clear console
                input = Console.ReadLine();
                Console.Clear();

                //if input = "c" end the loop and go back to main menu else get next ip
                if (!input.Equals("c"))
                {
                    Console.WriteLine(breadCrumb);
                    Console.WriteLine();
                    Console.WriteLine("ip plox");
                    input = Console.ReadLine();
                }
            }
            //remove text from breadcrumb
            breadCrumb.Remove(breadCrumb.Length - 22, 22);
        }

        /// <summary>
        /// show subnetting calulator
        /// </summary>
        private void EqualSubnetting()
        {
            //clear console and write breadcrumb
            Console.Clear();
            breadCrumb.Append(" -> Equal Subnetting");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();

            //declare vars
            IPAddress ipAdress;
            IPAddress subnetMask_1;
            IPAddress subnetMask_2;

            //ask for ip and get input
            Console.WriteLine("ip plox");
            string input = Console.ReadLine();

            while (input != "c")
            {
                //validate and parse input
                ipAdress = IPAddress.Parse(inputIPAddress(input));
                //ask for subnetmast and get input
                Console.WriteLine("subnetmask plox");
                input = Console.ReadLine();
                //checks if cidr or mask is given
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

                //ask for number of networks and get number
                Console.WriteLine("How many networks do you want to create?");
                input = Console.ReadLine();
                int aantNetwerken = Int32.Parse(input);
                //get number of extra bits
                int extraBits = toBitsNeeded(aantNetwerken);
                //get number of original bits
                int originalBits = getLengthFromMask(subnetMask_1);
                //calculate new length
                int newLength = originalBits + extraBits;
                //get new subnetmask
                subnetMask_2 = new IPAddress(getMaskFromLength(newLength));

                //for debugging
                //Console.WriteLine("original length = " + originalBits);
                //Console.WriteLine("extra bits = " + extraBits);
                //Console.WriteLine("new subnet = " + newLength);
                //Console.WriteLine("new subnet = /" + newLength);
                //Console.WriteLine("new subnetmask = " + subnetMask_2);

                //make array to store networks in
                IPAddress[] netwerken = new IPAddress[aantNetwerken];

                //calculate the first address
                netwerken[0] = ipAdress;
                //determine witch octet needs to be modified
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

                //store ip in decimal format in an array
                byte[] bytes = new byte[4];
                int[] ints = new int[4];

                //for debugging
                //Console.WriteLine("octet = " + octet);

                //get position of last bit in octet
                int positie = newLength - (octet - 1) * 8;

                //for debugging
                //Console.WriteLine("positie = " + positie);

                //invert position for calulations
                positie = 9 - positie;

                //for debugging
                //Console.WriteLine("positie = " + positie);

                //claclulate value of bit
                int waarde = (int)Math.Pow(2, positie - 1);

                //for debugging
                //Console.WriteLine("waarde = " +  waarde);

                //prepare console window to chow results
                Console.Clear();
                Console.WriteLine(breadCrumb);
                Console.WriteLine();

                //show results for first subnet
                Console.WriteLine("1  : {0,-15}/{1}", ipAdress, newLength);
                //calcuate the rest uf the subnets in a loop
                for (int i = 1; i < aantNetwerken; i++)
                {

                    bytes = netwerken[i - 1].GetAddressBytes();
                    ints = Array.ConvertAll(bytes, Convert.ToInt32);

                    //if (Convert.ToInt32(bytes[octet -1]) + waarde <= 255)
                    //{
                    //    bytes[octet - 1] = Convert.ToByte(Convert.ToInt32(bytes[octet - 1]) + waarde);
                    //}
                    //else
                    //{
                    //    bytes[octet - 1] = Convert.ToByte(0);
                    //    bytes[octet - 2] = Convert.ToByte(Convert.ToInt32(bytes[octet - 2]) + 1);
                    //}

                    ints[octet - 1] = ints[octet - 1] + waarde;

                    //the next three loops handle overflow to next octet
                    while (ints[3] > 255)
                    {
                        ints[2] = ints[2] + 1;
                        ints[3] = ints[3] - 256;
                    }
                    while (ints[2] > 255)
                    {
                        ints[1] = ints[1] + 1;
                        ints[2] = ints[2] - 256;
                    }
                    while (ints[1] > 255)
                    {
                        ints[0] = ints[0] + 1;
                        ints[1] = ints[1] - 256;
                    }

                    bytes = Array.ConvertAll(ints, Convert.ToByte);

                    netwerken[i] = new IPAddress(bytes);
                    Console.WriteLine("{0, -3}: {1, -15}/{2}", i + 1, netwerken[i], newLength);
                }

                //get input and clear console
                input = Console.ReadLine();
                Console.Clear();

                //if input = "c" end the loop and go back to main menu else get next ip
                if (!input.Equals("c"))
                {
                    Console.WriteLine(breadCrumb);
                    Console.WriteLine();
                    Console.WriteLine("ip plox");
                    input = Console.ReadLine();
                }
            }
            //remove last part of breadcrumb
            breadCrumb.Remove(breadCrumb.Length - 20, 20);
        }

        /// <summary>
        /// calulate vlsm
        /// </summary>
        private void VLSM()
        {
            //clear console and draw breadcrumb
            Console.Clear();
            breadCrumb.Append(" -> VLSM");
            Console.WriteLine(breadCrumb);
            Console.WriteLine();
            StringBuilder output = new StringBuilder();

            //declare vars for ip and subnetmask
            IPAddress baseIpAdress;
            IPAddress baseSubnetMask;

            //create array to store the number of hosts foe each network
            int[] netwerken;

            //ask for ip and get input
            Console.WriteLine("ip plox");
            string input = Console.ReadLine();

            //do calculations until user exits
            while (input != "c" || input != "se")
            {
                //get ip address form input
                baseIpAdress = IPAddress.Parse(inputIPAddress(input));
                //ask for subnetmask and get input
                Console.WriteLine("subnetmask plox");
                input = Console.ReadLine();
                //check if decimal or cidr notation is given and get subnetmask
                if (input.StartsWith("/"))
                {
                    int netPartLength = Int32.Parse(input.Substring(1, input.Length - 1));
                    baseSubnetMask = new IPAddress(getMaskFromLength(netPartLength));
                    //Console.WriteLine("subnetmask = " + baseSubnetMask);
                    //Console.WriteLine("networkAddress = " + baseIpAdress.GetNetworkAddress(baseSubnetMask));
                    //Console.WriteLine("broadcastAddress = " + baseIpAdress.GetBroadcastAddress(baseSubnetMask));

                }
                else
                {
                    baseSubnetMask = IPAddress.Parse(inputIPAddress(input));
                    //Console.WriteLine("slashNotation = /" + getLengthFromMask(baseSubnetMask));
                    //Console.WriteLine("networkAddress = " + baseIpAdress.GetNetworkAddress(baseSubnetMask));
                    //Console.WriteLine("broadcastAddress = " + baseIpAdress.GetBroadcastAddress(baseSubnetMask) + "\n");
                }

                //ask for the number of hosts in each network
                Console.WriteLine("Give the amount of hosts for each network, separated whith a space");
                input = Console.ReadLine();

                //prepare console to show results
                Console.Clear();
                Console.WriteLine(breadCrumb);
                Console.WriteLine();

                //Console.WriteLine("Given range: {0}/{1}", baseIpAdress, getLengthFromMask(baseSubnetMask));
                //Console.WriteLine();
                output.AppendFormat("Given range: {0}/{1}\n\n", baseIpAdress, getLengthFromMask(baseSubnetMask));

                //store inputs in array
                netwerken = Array.ConvertAll(input.Split(' '), Int32.Parse);

                //sort array
                Array.Sort(netwerken);
                Array.Reverse(netwerken);

                //get decimal value of octets and store in array
                int[] address = Array.ConvertAll(baseIpAdress.GetAddressBytes(), Convert.ToInt32);

                //do calulations in loop
                for (int i = 0; i < netwerken.Length; i++)
                {
                    //sets var to store new mask
                    int mask = 0;
                    //sets var to store number to be added to last octet
                    int addNum = 0;
                    //sets a variable to be used as the mask value.
                    int maskVar = 30;
                    //get number of hosts for current network  
                    int n = netwerken[i];
                    //calculate mask and number to add to ip
                    for (int j = 1; j < 28; j++)
                    {
                        if (n > Math.Pow(2, j) - 2)
                        {  //checks to see if the current mask value is sufficient to meet the current network's requirements.
                            mask = maskVar;
                            addNum = (int)Math.Pow(2, (j + 1));  //figures out how much to add to the address to get the next network address.
                        }
                        maskVar = maskVar - 1;  //drops the mask value by one for the next time through the loop.
                    }

                    //store ip in IPAddress type
                    IPAddress networkAddress = new IPAddress(Array.ConvertAll(address, Convert.ToByte));
                    //show results
                    //Console.WriteLine("{0, -8}: {1, -15}/{2}", n, networkAddress, mask);
                    output.AppendFormat("{0, -8}: {1, -15}/{2}\n", n, networkAddress, mask);
                    
                    //add number to last octet
                    address[3] = address[3] + addNum;

                    //these loops handle overflow to prev octet 
                    while (address[3] > 255)
                    {
                        address[2] = address[2] + 1;
                        address[3] = address[3] - 256;
                    }
                    while (address[2] > 255)
                    {
                        address[1] = address[1] + 1;
                        address[2] = address[2] - 256;
                    }
                    while (address[1] > 255)
                    {
                        address[0] = address[0] + 1;
                        address[1] = address[1] - 256;
                    }
                }

                Console.Write(output);

                //get input
                input = Console.ReadLine();

                //stop calculation if input = "c" and go back to main menu
                if (!input.Equals("c"))
                {
                    if (!input.Equals("se"))
                    {
                        Console.WriteLine("ip plox");
                        input = Console.ReadLine();
                        //prepare console for next calculations
                        Console.Clear();
                        Console.WriteLine(breadCrumb);
                        Console.WriteLine();
                    }
                    //save output to file
                    string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VLSM.txt");
                    Console.WriteLine("Writing to: " + destPath);
                    File.WriteAllText(destPath, output.ToString());
                }
            }

            //remove part from breadcrumb
            breadCrumb.Remove(breadCrumb.Length - 8, 8);

        }

        /// <summary>
        /// Checks if input is valid
        /// </summary>
        /// <param name="aant">Max number of options</param>
        /// <returns>valid option</returns>
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

        /// <summary>
        /// validate ip address from string
        /// </summary>
        /// <param name="input">string to validate</param>
        /// <returns>valid ip address</returns>
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

        /// <summary>
        /// Gives the number of bits needed to represent a decimal number
        /// </summary>
        /// <param name="num">Decimal number</param>
        /// <returns>number of bits needed to represent number</returns>
        public static int toBitsNeeded(int num)
        {
            return (int)(Math.Log(num, 2));
        }

        public static string inputIPAddress()
        {
            string input = Console.ReadLine();
            return inputIPAddress(input);
        }

        /// <summary>
        /// Checks if input is a valid ip address.
        /// </summary>
        /// <param name="ipString">ipString</param>
        /// <returns>True if valid</returns>
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

        /// <summary>
        /// Get subnetmask from cidr
        /// </summary>
        /// <param name="netPartLength">cidr</param>
        /// <returns>byte array of octets</returns>
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

        /// <summary>
        /// Get cidr of subnetmask
        /// </summary>
        /// <param name="subnetMask">subnetMask</param>
        /// <returns>cidr</returns>
        public static int getLengthFromMask(IPAddress subnetMask)
        {
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
