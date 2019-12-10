using System;

namespace Blinky
{
    /// <summary>
    /// GPIO: Representation of the general purpose I/Os
    /// </summary>
    internal class GPIO
    {
        private bool lamp = false;

        /// <summary>
        /// Gets or sets a value indicating whether the lamp is switched on or off.
        /// </summary>
        /// <value>
        ///   <c>true</c> if lamp in switched on; otherwise, <c>false</c>.
        /// </value>
        public bool Lamp
        {
            get => this.lamp;
            set
            {
                this.lamp = value;
                if (this.lamp)
                    Console.WriteLine("Lamp switched on");
                else
                    Console.WriteLine("Lamp switched off");
            }
        }

        /// <summary>
        /// Toggles the lamp.
        /// </summary>
        public void ToggleLamp()
        {
            this.Lamp = !this.Lamp;
        }
    }
}
