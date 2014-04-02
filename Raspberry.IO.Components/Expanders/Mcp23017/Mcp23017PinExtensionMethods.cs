namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public static class Mcp23017PinExtensionMethods
    {
        public static Mcp23017OutputBinaryPin Out(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017OutputBinaryPin(connection, pin, resistor, polarity);
        }        
        
        public static Mcp23017InputBinaryPin In(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017InputBinaryPin(connection, pin, resistor, polarity);
        }
    }
}