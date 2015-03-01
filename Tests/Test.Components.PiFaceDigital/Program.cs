using System;
using System.Threading;
using System.Threading.Tasks;
using Raspberry.IO.Components.Devices.PiFaceDigital;


namespace Test.Components.PiFaceDigital
{
    class Program
    {
        private static PiFaceDigitalDevice piFace;
        private static bool polling = true;
        private static Task t;

        /// <summary>
        /// Demo App
        /// 
        /// Loops through each output pin 10 times pulsing them on for 1/2 second
        /// Inputs are polled and any change reported on the console
        /// </summary>
        static void Main()
        {

            piFace = new PiFaceDigitalDevice();


            // setup events
            foreach (var ip in piFace.InputPins)
            {
                ip.OnStateChanged += ip_OnStateChanged;
            }

            t = Task.Factory.StartNew(() => PollInputs());

            for (int i = 0; i < 10; i++)
            {
                for (int pin = 0; pin < 8; pin++)
                {
                    piFace.OutputPins[pin].State = true;
                    piFace.UpdatePiFaceOutputPins();
                    Thread.Sleep(500);
                    piFace.OutputPins[pin].State = false;
                    piFace.UpdatePiFaceOutputPins();
                    Thread.Sleep(500);
                }
            }

            //stop polling loop
            polling = false;
            t.Wait();
        }




        /// <summary>
        /// Loop polling the inputs at 200ms intervals
        /// </summary>
        private static void PollInputs()
        {
            while (polling)
            {
                piFace.PollInputPins();
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Log any input pin changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ip_OnStateChanged(object sender, InputPinChangedArgs e)
        {
            Console.WriteLine("Pin {0} became {1}", e.pin.Id, e.pin.State);
        }
    }
}
