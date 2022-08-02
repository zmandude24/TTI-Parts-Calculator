namespace TTI_Calculations
{
    public class Resistor
    {
        public double R = 0;
        public double powRating = 0;
        public double cost = 0;
        public string tag = "";

        public Resistor(double R, double powRating = 0, double cost = 0,
            string tag = "")
        {
            if (R > 0)
                this.R = R;
            if (powRating > 0)
                this.powRating = powRating;
            if (cost > 0)
                this.cost = cost;
            if (tag != "")
                this.tag = tag;
        }
    }

    public class Capacitor
    {
        public double C = 0;
        public double cost = 0;
        public string tag = "";

        public Capacitor(double C, double cost = 0, string tag = "")
        {
            if (C > 0)
                this.C = C;
            if (cost > 0)
                this.cost = cost;
            if (tag != "")
                this.tag = tag;
        }
    }

    public class ADC
    {
        public Resistor R1;
        public Resistor R2;
        public Capacitor C;
        public double totalCost = double.PositiveInfinity;

        public ADC()
        {
            R1 = new Resistor(0);
            R2 = new Resistor(0);
            C = new Capacitor(0);
        }
    }

    public static class ADC_Func
    {
        public static double Rz =           // Leakage is 15uA @ 1V
            1 / (15 * Math.Pow(10, -6));
        public static double R1divR2 = 46.7 / 3.3;  // R1 / R2'
        public static double maxError = 0.001;
        public static double fs = 5000;

        public static double GetR1(Resistor R2)
        {
            if (R2 == null)
            {
                Console.WriteLine("Error: R2 is null.");
                return 0;
            }
            if (R2.R == 0)
            {
                Console.WriteLine("Error: R2 resistance not set.");
                return 0;
            }

            double R2prime = R2.R * Rz / (R2.R + Rz);
            return R2prime * R1divR2;
        }
        public static double GetR2(Resistor R1)
        {
            if (R1 == null)
            {
                Console.WriteLine("Error: R1 is null.");
                return 0;
            }
            if (R1.R == 0)
            {
                Console.WriteLine("Error: R1 resistance not set.");
                return 0;
            }

            double R2prime = R1.R / R1divR2;
            return R2prime * Rz / (Rz - R2prime);
        }
        public static double GetC(Resistor R1, Resistor R2)
        {
            if ((R1 == null) || (R2 == null))
            {
                if (R1 == null)
                    Console.WriteLine("Error: R1 is null.");
                if (R2 == null)
                    Console.WriteLine("Error: R2 is null.");
                return 0;
            }
            if ((R1.R == 0) || (R2.R == 0))
            {
                if (R1.R == 0)
                    Console.WriteLine("Error: R1 resistance not set.");
                if (R2.R == 0)
                    Console.WriteLine("Error: R2 resistance not set.");
                return 0;
            }

            double R2prime = R1.R / R1divR2;
            return (R1.R + R2prime) / (R1.R * R2prime) * (1 / fs);
        }

        public static double GetRfromString(string? input)
        {
            if (input == null) { return 0; }

            double R;

            // Just a number
            if (input[input.Length - 1] != 'O')
            {
                try
                {
                    R = Convert.ToDouble(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    R = 0;
                }
            }

            // With unit name
            else
            {
                char prefix = input[input.Length - 2];
                string value = input.Substring(0, input.Length - 2);

                try
                {
                    switch (prefix)
                    {
                        case 'M':
                            R = Convert.ToDouble(value)
                                * Math.Pow(10, 6);
                            break;
                        case 'k':
                            R = Convert.ToDouble(value)
                                * Math.Pow(10, 3);
                            break;
                        case 'm':
                            R = Convert.ToDouble(value)
                                * Math.Pow(10, -3);
                            break;
                        case 'u':
                            R = Convert.ToDouble(value)
                                * Math.Pow(10, -6);
                            break;
                        default:
                            value = input.Substring(0, input.Length - 1);
                            R = Convert.ToDouble(value);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    R = 0;
                }
            }

            return R;
        }
        public static double GetCfromString(string? input)
        {
            if (input == null) { return 0; }

            double C;

            // Just a number
            if (input[input.Length - 1] != 'F')
            {
                try
                {
                    C = Convert.ToDouble(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    C = 0;
                }
            }

            // With unit name
            else
            {
                char prefix = input[input.Length - 2];
                string value = input.Substring(0, input.Length - 2);

                try
                {
                    switch (prefix)
                    {
                        case 'm':
                            C = Convert.ToDouble(value)
                                * Math.Pow(10, -3);
                            break;
                        case 'u':
                            C = Convert.ToDouble(value)
                                * Math.Pow(10, -6);
                            break;
                        case 'p':
                            C = Convert.ToDouble(value)
                                * Math.Pow(10, -12);
                            break;
                        default:
                            value = input.Substring(0, input.Length - 1);
                            C = Convert.ToDouble(value);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    C = 0;
                }
            }

            return C;
        }

        public static string GetRasString(double R, int sigFigs = 5)
        {
            if (sigFigs <= 0)
            {
                Console.WriteLine("Error in GetRasString: "
                    + "sigFigs must be a positive integer.");
                return "";
            }

            string unitName = "";
            double RinNewUnits = 0;
            int decimalPlace = (int)Math.Floor(Math.Log10(R));

            /* set unitname and adjust the decimal place */
            if (decimalPlace >= 6)
            {
                unitName = "MO";
                RinNewUnits = R / Math.Pow(10, 6);
                decimalPlace -= 6;
            }
            else if (decimalPlace >= 3)
            {
                unitName = "kO";
                RinNewUnits = R / Math.Pow(10, 3);
                decimalPlace -= 3;
            }
            else if (decimalPlace >= 0)
            {
                unitName = "O";
                RinNewUnits = R;
            }
            else if (decimalPlace >= -3)
            {
                unitName = "mO";
                RinNewUnits = R / Math.Pow(10, -3);
                decimalPlace -= -3;
            }
            else if (decimalPlace >= -6)
            {
                unitName = "uO";
                RinNewUnits = R / Math.Pow(10, -6);
                decimalPlace -= -6;
            }
            else return "0";

            /* Round the adjusted value */
            if (decimalPlace > sigFigs)
            {
                RinNewUnits *= Math.Pow(10, -(sigFigs - decimalPlace));
                RinNewUnits = Math.Round(RinNewUnits, sigFigs - 1);
                RinNewUnits *= Math.Pow(10, sigFigs - decimalPlace);
            }
            else
                RinNewUnits = Math.Round(RinNewUnits,
                    sigFigs - decimalPlace - 1);

            return Convert.ToString(RinNewUnits) + unitName;
        }
        public static string GetCasString(double C, int sigFigs = 5)
        {
            if (sigFigs <= 0)
            {
                Console.WriteLine("Error in GetCasString: "
                    + "sigFigs must be a positive integer.");
                return "";
            }

            string unitName = "";
            double CinNewUnits = 0;
            int decimalPlace = (int)Math.Floor(Math.Log10(C));

            /* set unitname and adjust the decimal place */
            if (decimalPlace >= -3)
            {
                unitName = "mF";
                CinNewUnits = C / Math.Pow(10, -3);
                decimalPlace -= -3;
            }
            else if (decimalPlace >= -8)
            {
                unitName = "uF";
                CinNewUnits = C / Math.Pow(10, -6);
                decimalPlace -= -6;
            }
            else if (decimalPlace >= -12)
            {
                unitName = "pF";
                CinNewUnits = C / Math.Pow(10, -12);
            }
            else return "0";

            /* Round the adjusted value */
            if (decimalPlace > sigFigs)
            {
                CinNewUnits *= Math.Pow(10, -(sigFigs - decimalPlace));
                CinNewUnits = Math.Round(CinNewUnits, sigFigs - 1);
                CinNewUnits *= Math.Pow(10, sigFigs - decimalPlace);
            }
            else
                CinNewUnits = Math.Round(CinNewUnits,
                    sigFigs - decimalPlace - 1);

            return Convert.ToString(CinNewUnits) + unitName;
        }

        public static void PrintRange(Resistor R, string rName,
            int sigFigs = 5)
        {
            if (R == null)
            {
                Console.WriteLine("Error: resistor is null.");
                return;
            }

            string Rmin = GetRasString((1 - maxError) * R.R, sigFigs);
            string Rmax = GetRasString((1 + maxError) * R.R, sigFigs);
            Console.WriteLine(Rmin + " <= " + rName + " <= " + Rmax);
        }

        public static double GetTotalCost(ADC adc)
        {
            if (adc == null)
            {
                Console.WriteLine("Error: adc is null.");
                return 0;
            }
            if ((adc.R1 == null) || (adc.R2 == null) || (adc.C == null))
            {
                if (adc.R1 == null)
                    Console.WriteLine("Error: R1 is null.");
                if (adc.R2 == null)
                    Console.WriteLine("Error: R2 is null.");
                if (adc.C == null)
                    Console.WriteLine("Error: C is null.");
                return 0;
            }
            if ((adc.R1.cost == 0) || (adc.R2.cost == 0)
                || (adc.C.cost == 0))
            {
                if (adc.R1.cost == 0)
                    Console.WriteLine("Error: R1 cost is not set.");
                if (adc.R2.cost == 0)
                    Console.WriteLine("Error: R2 cost is not set.");
                if (adc.C.cost == 0)
                    Console.WriteLine("Error: C cost is not set.");
                return 0;
            }

            return adc.R1.cost + adc.R2.cost + adc.C.cost;
        }
    }
    
    class Program
    {
        private static void ListCommands(string[] commandList)
        {
            Console.WriteLine("Commands:");
            foreach (string command in commandList)
                Console.WriteLine(command);
        }

        private static void PrintADC(ADC adc)
        {
            Console.WriteLine("Current ADC:");
            PrintResistor(adc.R1, "R1");
            PrintResistor(adc.R2, "R2");
            PrintCapacitor(adc.C);
            if (adc.totalCost == double.PositiveInfinity)
                Console.WriteLine("Total Cost is not set.");
            else
                Console.WriteLine("Total Cost = $"
                    + Math.Round(adc.totalCost, 2));
        }
        private static void PrintResistor(Resistor R, string rName)
        {
            Console.WriteLine(rName + ":");

            if (R.R == 0)
                Console.WriteLine("R is not set.");
            else
                Console.WriteLine("R = " + ADC_Func.GetRasString(R.R));

            if (R.powRating == 0)
                Console.WriteLine("Power Rating is not set.");
            else
                Console.WriteLine("Power Rating = " + R.powRating + "W");

            if (R.cost == 0)
                Console.WriteLine("Cost is not set.");
            else
                Console.WriteLine("Cost = $" + Math.Round(R.cost, 2));

            if (R.tag == "")
                Console.WriteLine("Tag is not set.");
            else
                Console.WriteLine("Tag: " + R.tag);
        }
        private static void PrintCapacitor(Capacitor C)
        {
            Console.WriteLine("C:");
            
            if (C.C == 0)
                Console.WriteLine("C is not set.");
            else
                Console.WriteLine("C = " + ADC_Func.GetCasString(C.C));

            if (C.cost == 0)
                Console.WriteLine("Cost is not set.");
            else
                Console.WriteLine("Cost = $" + Math.Round(C.cost, 2));

            if (C.tag == "")
                Console.WriteLine("Tag is not set.");
            else
                Console.WriteLine("Tag: " + C.tag);
        }

        static void Main(string[] args)
        {
            List<ADC> adcList = new List<ADC>();
            ADC adc = new ADC();
            string[] commandList = { "?", "quit", "print",
                "set R1", "set R2", "set C" };

            while (true)
            {
                Console.Write("Enter a command: ");
                string? command = Console.ReadLine();

                if (Array.IndexOf(commandList, command) == -1)
                {
                    Console.WriteLine("Type ? for the list of valid "
                        + "commands.");
                }
                else if (command == "?")
                {
                    ListCommands(commandList);
                }
                else if (command == "quit")
                {
                    break;
                }
                else if (command == "print")
                {
                    PrintADC(adc);
                }
                else if (command == "set R1")
                {
                    Console.Write("R1 = ");
                    string? input = Console.ReadLine();
                    double dummy = ADC_Func.GetRfromString(input);

                    Console.Write("Rating(W): ");
                    input = Console.ReadLine();
                    try
                    {
                        adc.R1.powRating = Convert.ToDouble(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Cost: $");
                    input = Console.ReadLine();
                    try
                    {
                        adc.R1.cost = Convert.ToDouble(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Tag: ");
                    input = Console.ReadLine();
                    if (input != null)
                        adc.R1.tag = input;

                    // Set other parameters if not done already
                    if (dummy != 0)
                    {
                        adc.R1.R = dummy;
                        if (adc.R2.R == 0)
                            adc.R2.R = ADC_Func.GetR2(adc.R1);
                        if (adc.C.C == 0)
                            adc.C.C = ADC_Func.GetC(adc.R1, adc.R2);
                    }
                    else
                    {
                        Console.WriteLine("Error: Failed to set " +
                            "a value.");
                    }
                }
                else if (command == "set R2")
                {
                    Console.Write("R2 = ");
                    string? input = Console.ReadLine();
                    double dummy = ADC_Func.GetRfromString(input);

                    Console.Write("Rating(W): ");
                    input = Console.ReadLine();
                    try
                    {
                        adc.R2.powRating = Convert.ToDouble(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Cost: $");
                    input = Console.ReadLine();
                    try
                    {
                        adc.R2.cost = Convert.ToDouble(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Tag: ");
                    input = Console.ReadLine();
                    if (input != null)
                        adc.R2.tag = input;

                    // set other parameters if not done already
                    if (dummy != 0)
                    {
                        adc.R2.R = dummy;
                        if (adc.R1.R == 0)
                            adc.R1.R = ADC_Func.GetR1(adc.R2);
                        if (adc.C.C == 0)
                            adc.C.C = ADC_Func.GetC(adc.R1, adc.R2);
                    }
                    else
                    {
                        Console.WriteLine("Error: Failed to set " +
                            "a value.");
                    }
                }
                else if (command == "set C")
                {
                    Console.Write("C = ");
                    string? input = Console.ReadLine();
                    adc.C.C = ADC_Func.GetCfromString(input);

                    Console.Write("Cost: $");
                    input = Console.ReadLine();
                    try
                    {
                        adc.C.cost = Convert.ToDouble(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.Write("Tag: ");
                    input = Console.ReadLine();
                    if (input != null)
                        adc.C.tag = input;

                    if (adc.C.C == 0)
                    {
                        Console.WriteLine("Error: Failed to set " +
                            "a value.");
                    }
                }
                else
                {
                    Console.WriteLine("Command not implemented");
                }
                Console.WriteLine("");
            }
        }
    }
}
