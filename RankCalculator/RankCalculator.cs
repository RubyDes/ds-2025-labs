using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RankCalculator started");

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = connectionMultiplexer.GetDatabase();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine("Preparing id " + id);

                string textKey = "TEXT-" + id;
                string text = db.StringGet(textKey);
                double rank = 0;
                if (text != null) {
                    int notLetterCharsCount = text.Where(ch => !char.IsLetter(ch)).Count();
                    rank = notLetterCharsCount / (double) text.Length;
                }

                string rankKey = "RANK-" + id;
                db.StringSet(rankKey, rank.ToString("0.##"));
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}
