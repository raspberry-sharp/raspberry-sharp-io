namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    public static class Mcp23017PinExtensionMethods
    {
        public static Mcp23017OutputPin Out(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017OutputPin(connection, pin, resistor, polarity);
        }        
        
        public static Mcp23017InputPin In(this Mcp23017I2cConnection connection, Mcp23017Pin pin, Mcp23017PinResistor resistor = Mcp23017PinResistor.None, Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            return new Mcp23017InputPin(connection, pin, resistor, polarity);
        }
    }
}