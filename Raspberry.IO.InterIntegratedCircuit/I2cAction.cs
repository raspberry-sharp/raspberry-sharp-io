namespace Raspberry.IO.InterIntegratedCircuit
{
    using System;

    public abstract class I2cAction
    {
        protected I2cAction(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            Buffer = buffer;
        }

        public byte[] Buffer { get; private set; }
    }
}
