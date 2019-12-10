using System;
using System.Diagnostics;
using System.Threading;

namespace Blinky
{

    class Program
    {
        private static BlinkySMState _blinkySM = null;  //the blinky-statemachine

        /// <summary>
        /// blinky-machine-thread
        /// </summary>
        private static void BlinkySMRunThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    _blinkySM.RunCycle();   //runs one cycle in state machine
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello Blinky-Machine!");

                //create the blinky-machine-instance
                _blinkySM = new BlinkySMState();
                _blinkySM.StatePathChanged += BlinkySM_StateChanged;

                //create GPIO-Handler (general purpose input/output): With this object the lamp is controlled. 
                GPIO gpio = new GPIO();
                _blinkySM.InjectVar("gpio", gpio);

                //create Timer-Object for workflow-timing
                Stopwatch timer = new Stopwatch();
                _blinkySM.InjectVar("timer", timer);
                
                //start run-thread
                new Thread(new ThreadStart(BlinkySMRunThread)) { IsBackground = true, Name = "Blinky-Run-Thread" }.Start();

                //command-input
                Console.WriteLine("please use the 'enable','disable' and 'toggle' command... ");
                string line = null;
                while (true)
                {
                    if ((line=Console.ReadLine()).Contains("enable"))
                        _blinkySM.InjectCommand(BlinkySMCommands.Enable);
                    if (line.Contains("disable"))
                        _blinkySM.InjectCommand(BlinkySMCommands.Disable);
                    if (line.Contains("toggle"))
                        _blinkySM.InjectCommand(EnabledStateCommands.ToggleBlinkSpeed);

                    if (line.Contains("exit"))
                        Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// event-handler for state-changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BlinkySM_StateChanged(object sender, EM.EMSm.StatePathChangedEventArgs e)
        {
            Console.WriteLine($"old:{e.OldStatePath} new:{e.NewStatePath}");
        }
    }
}
