using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dim;
using System.Timers;
using DarkRift.Server;
using DarkRift;

namespace Dim.GameTypes
{
    public class DamageDeliveryGameMachine
    {
        List<PlayerInputs> playerWhoPlayList;
        List<PlayerInputs> playerWhoWaitForPlayList;
        Dictionary<IClient, PlayerInputs> players;
        State actualState;


        public DamageDeliveryGameMachine(Dictionary<IClient, PlayerInputs> pla)
        {
            playerWhoPlayList = new List<PlayerInputs>();
            playerWhoWaitForPlayList = new List<PlayerInputs>();
            players = pla;

            State endGameState = new EndMatchState(10000);
            endGameState.Init(this, players);
            State matchState = new MatchState(60000);
            matchState.Init(this, players);
            State beforGameState = new BeforeGameState(10000);
            beforGameState.Init(this, players);

           // State initState = new InitState();
           // initState.Init(this, players);
          //  initState.SetState(beforGameState);



            beforGameState.SetState(matchState);
            matchState.SetState(endGameState);
            endGameState.SetState(beforGameState);

            actualState = beforGameState;
            
        }

        public void ChangeState(State to)
        {
            actualState = to;
        }

        public void Update()
        {
            actualState.Decide();
        }
    }

    public interface State
    {
        void Init(DamageDeliveryGameMachine mach, Dictionary<IClient, PlayerInputs> plrs);
        void SetState(State state);
        void Decide();


    }

     abstract class AbstractState : State
    {
        protected DamageDeliveryGameMachine stateMachine;
        protected State nextState;
        protected Dictionary<IClient, PlayerInputs> players;
        PlayerInputs input;

        public virtual void Decide()
        {
            
        }

        public void Init(DamageDeliveryGameMachine mach, Dictionary<IClient, PlayerInputs> plrs)
        {
            stateMachine = mach;
            players = plrs;
        }

        public void SetState(State state)
        {
            nextState = state;
        }
    }


    class InitState : AbstractState
    {
        public override void Decide()
        {
            stateMachine.ChangeState(nextState);
        }
    }

    class BeforeGameState : AbstractState
    {
        
        protected bool started = false;
        protected Timer timer;
        protected double interval = 10000;
        protected string stateName;
        //int ind = 0;

        public BeforeGameState(double breakTime)
        {
            interval = breakTime;
            timer = new Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.Stop();
            stateName = "Before match state";
        }

        public override void Decide()
        {
            if (players.Count > 1)
            {
                if (!started)
                {
                    // ind++;
                    // Console.WriteLine("Start " + stateName + "    " + ind);
                    SendMessageFromDeliveryDamage.SendGameBreakStartInfo( players);
                    foreach (PlayerInputs pi in players.Values)
                    {
                        SendMessageFromDeliveryDamage.SendDamageDelivery(pi, players);
                    }
                    timer.Start();
                    started = true;
                }
            }
            else
            {
                timer.Stop();
               // timer.Interval += interval;
                started = false;
               // Console.WriteLine("Czas " + timer.Interval);
            }
        }

        protected virtual void SomethingToDo()
        {

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            started = false;
            timer.Stop();
           // timer.Interval += interval;
           // timer.
            SomethingToDo();
            stateMachine.ChangeState(nextState);

           //Console.WriteLine("Next state " + nextState.ToString());
           // Console.WriteLine("End " + stateName);
        }
    }

     class MatchState :BeforeGameState
    {

        public MatchState(double breakTime) : base(breakTime)
        {
            stateName = " Match State ";
            //timer.Start();
        }

        public override void Decide()
        {
            foreach (PlayerInputs pI in players.Values)
            {
                if (pI.Change)
                {
                    SendMessageFromDeliveryDamage.SendDamageDelivery(pI, players);
                }
            }

            if (players.Count < 2)
            {
                timer.Stop();
                //timer.Interval += interval;
                stateMachine.ChangeState(nextState);
                
                started = false;
               // Console.WriteLine("End  " + stateName + " ++++");
            }
            else
            {
                if (!started)
                {
                    SendMessageFromDeliveryDamage.SendGameMatchStartInfo(players);
                    foreach (PlayerInputs pi in players.Values)
                    {
                        SendMessageFromDeliveryDamage.SendDamageDelivery(pi, players);
                    }
                    // Console.WriteLine("Start " + stateName);
                    started = true;
                    timer.Start();
                }
            }
           // base.Decide();
        }
    }

     class EndMatchState :BeforeGameState
    {
        public EndMatchState(double breakTime) : base(breakTime)
        {
           // stateName = "End match state ";
        }

        protected override void SomethingToDo()
        {

            //wyrzuc gracza który przegrał a raczej przerzuc go do graczy oczekujących a zwloki, kontroller wyrzuc i wsadz następnego gracza. Jeśli nie ma następnego gracz nie rób nic
            base.SomethingToDo();
        }

        public override void Decide()
        {


            if (players.Count < 1)
            {
                timer.Stop();
                //timer.Interval += interval;
                stateMachine.ChangeState(nextState);
                started = false;
               // Console.WriteLine("End " + stateName + " ++++");
            }
            else
            {
                if (!started)
                {
                    SendMessageFromDeliveryDamage.SendGameEndStartInfo(players);

                    // Console.WriteLine("Start " + stateName);
                    started = true;
                    timer.Start();
                }

            }
           // base.Decide();
        }
    }

    public static class SendMessageFromDeliveryDamage
    {
        public static void SendDamageDelivery(PlayerInputs pi, Dictionary<IClient, PlayerInputs> dic)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(pi.ID);
                writer.Write(pi.DamageDeliverdd());
                writer.Write(pi.DamageTaken());

                

                using (Message damage = Message.Create(DimTag.DeliveryDamageInfo, writer))
                {
                    foreach (IClient client in dic.Keys.ToArray())
                    {
                        //if(client.ID == pi.ID)
                        client.SendMessage(damage, SendMode.Reliable);
                    }
                }
            }          
        }

        public static void SendGameBreakStartInfo(Dictionary<IClient, PlayerInputs> dic)
        {
            foreach (PlayerInputs pi in dic.Values)
                pi.ResetDamage();

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                using (Message msg = Message.Create(DimTag.DeliveryDamageBreakStart, writer))
                {
                    foreach (IClient client in dic.Keys.ToArray())
                        client.SendMessage(msg, SendMode.Reliable);
                }
            }
        }


        public static void SendGameMatchStartInfo(Dictionary<IClient, PlayerInputs> dic)
        {
            foreach (PlayerInputs pi in dic.Values)
                pi.ResetDamage();
            //Console.WriteLine("us");
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                using (Message msg = Message.Create(DimTag.DeliveryDamageMatchStart, writer))
                {
                    foreach (IClient client in dic.Keys.ToArray())
                    {
                        client.SendMessage(msg, SendMode.Reliable);
                       // Console.WriteLine("prze");
                    }
                }
            }
        }

        public static void SendGameEndStartInfo(Dictionary<IClient, PlayerInputs> dic)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                using (Message msg = Message.Create(DimTag.DeliveryDamageMatchEndStart, writer))
                {
                    foreach (IClient client in dic.Keys.ToArray())
                        client.SendMessage(msg, SendMode.Reliable);
                }
            }
        }
    }

}
