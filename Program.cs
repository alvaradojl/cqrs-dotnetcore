using System;
using System.Collections.Generic;

namespace cqrs_dotnetcore
{

    public class Person
    {
        private int _age = 0;
        EventBroker _broker;

        public Person(EventBroker broker)
        {
            this._broker = broker;
            broker.Commands += BrokerOnCommands;
            broker.Queries += BrokerOnQueries;
        }

        private void BrokerOnCommands(object sender, Command command)
        {
            var cac = command as ChangeAgeCommand;
            if (cac != null && cac.Target == this)
            {
                _age = cac.Age;
            }
        }

        private void BrokerOnQueries(object sender, Query query)
        {
            var ac = query as AgeQuery;
            if (ac != null && ac.Target == this)
            {
                ac.Result = _age;
            }
        }

    }

    public class EventBroker
    {
        //All events
        public IList<Event> AllEvents = new List<Event>();

        //Commands
        public event EventHandler<Command> Commands;

        //Queries
        public event EventHandler<Query> Queries;

        public void Command(Command c)
        {
            Commands?.Invoke(this, c);
        }

        public T Query<T>(Query q)
        {
            Queries?.Invoke(this, q);
            return (T)q.Result;
        }
    }

    public class Command : EventArgs
    {

    }

    class ChangeAgeCommand : Command
    {
        public Person Target;
        public int Age;

        public ChangeAgeCommand(Person target, int age)
        {
            Target = target;
            Age = age;
        }

    }

    public class Event
    {

    }

    public class Query
    {
        public object Result;
    }

    class AgeQuery : Query
    {
        public Person Target;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var eb = new EventBroker();

            var p = new Person(eb);
            Console.WriteLine("Sending command to change age to 12...");
            eb.Command(new ChangeAgeCommand(p, 12));
            Console.WriteLine("Sending query about age...");
            int age = eb.Query<int>(new AgeQuery() { Target = p });

            Console.WriteLine($"aged changed to: {age}");


        }
    }
}
