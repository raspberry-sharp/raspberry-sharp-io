namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public static class Mcp23017PinExtensionMethods
    {
        public static Mcp23017OutputPin Out(this Mcp23017I2cConnection connection, Mcp23017Pin pin)
        {
            return new Mcp23017OutputPin(connection, pin);
        }
    }
}