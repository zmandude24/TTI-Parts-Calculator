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
        public double powRating = 0;
        public double cost = 0;
        public string tag = "";

        public Capacitor(double C, double powRating = 0, double cost = 0,
            string tag = "")
        {
            if (C > 0)
                this.C = C;
            if (powRating > 0)
                this.powRating = powRating;
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
        public double cost = double.PositiveInfinity;

        public ADC()
        {
            R1 = new Resistor(0);
            R2 = new Resistor(0);
            C = new Capacitor(0);
        }
    }

    public static class ADC_Func
    {
        public static double Rz =           // Leakage current is 15uA @ 1V
            1 / (15 * Math.Pow(10, -6));
        public static double R1divR2 = 46.7 / 3.3;  // R1 / R2'
        public static double maxError = 0.001;

        /* Get functions will return 0 on failure */
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
        public static double GetC(Resistor R1, Resistor R2,
            double fs)
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
                RinNewUnits = Math.Round(RinNewUnits, sigFigs - 1);

            return Convert.ToString(RinNewUnits) + unitName;
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