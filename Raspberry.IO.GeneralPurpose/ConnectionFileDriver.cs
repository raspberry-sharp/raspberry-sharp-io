using System.IO;

namespace Raspberry.IO.GeneralPurpose
{
    public class ConnectionFileDriver : IConnectionDriver
    {
        private const string gpioPath = "/sys/class/gpio";
        
        public void Write(ProcessorPin pin, bool value)
        {
            var gpioId = string.Format("gpio{0}", (int) pin);
            var filePath = Path.Combine(gpioId, "value");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(value ? "1" : "0");
        }

        public bool Read(ProcessorPin pin)
        {
            var gpioId = string.Format("gpio{0}", (int) pin);
            var filePath = Path.Combine(gpioId, "value");

            using (var streamReader = new StreamReader(new FileStream(Path.Combine(gpioPath, filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var rawValue = streamReader.ReadToEnd();
                return !string.IsNullOrEmpty(rawValue) && rawValue[0] == '1';
            }
        }

        public void Export(PinConfiguration pin)
        {
            var gpioId = string.Format("gpio{0}", (int) pin.Pin);
            if (Directory.Exists(Path.Combine(gpioPath, gpioId)))
                Unexport(pin);

            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "export"), false))
                streamWriter.Write((int) pin.Pin);

            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, filePath), false))
                streamWriter.Write(pin.Direction == PinDirection.Input ? "in" : "out");
        }

        public void Unexport(PinConfiguration pin)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(gpioPath, "unexport"), false))
                streamWriter.Write((int) pin.Pin);
        }
    }
}