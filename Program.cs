using System.Net.Mime;
namespace Rougelike
{
    internal class Program
    {
        public static Random rng = new Random();

        static void Main(){ //Starting function
            Dungeon d = new Dungeon();
            d.start();
        }

        internal static int roll(int number, int dice){ //Generating random number using dice system. 
            int sum = 0;
            for(int i = 0; i < number; i++){
                sum += rng.Next(1, dice+1);
            }
            return sum;
        }
        internal static int roll(int number, int dice, int min){    //Generating random number using dice system with custom minimal value per dice. 
            int sum = 0;
            for(int i = 0; i < number; i++){
                sum += rng.Next(min, dice+1);
            }
            return sum;
        }

        internal static void exit(float score) {    //Exiting the program
            System.Console.WriteLine("Score achieved: "+Math.Floor(score*100)/100);
            System.Console.ReadLine();
            Environment.Exit(0);
        }
    }
}