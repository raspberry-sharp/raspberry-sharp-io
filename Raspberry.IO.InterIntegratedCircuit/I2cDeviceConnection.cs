namespace Raspberry.IO.InterIntegratedCircuit
{
    public class I2cDeviceConnection
    {
        private readonly I2cDriver driver;
        private readonly int deviceAddress;

        internal I2cDeviceConnection(I2cDriver driver, int deviceAddress)
        {
            this.driver = driver;
            this.deviceAddress = deviceAddress;
        }

        public int DeviceAddress
        {
            get { return deviceAddress; }
        }

        public void Write(params byte[] buffer)
        {
            driver.Write(deviceAddress, buffer);
        }

        public void WriteByte(byte value)
        {
            Write(value);
        }

        public byte[] Read(int length)
        {
            return driver.Read(deviceAddress, length);
        }

        public byte ReadByte()
        {
            return Read(1)[0];
        }
    }
}