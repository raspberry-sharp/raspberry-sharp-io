namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connector pin.
    /// </summary>
    public enum ConnectorPin
    {
        #region Raspberry Pi connectors

        /// <summary>
        /// Connector P1, pin 3.
        /// </summary>
        P1Pin3,

        /// <summary>
        /// Connector P1, pin 3.
        /// </summary>
        P1Pin03 = P1Pin3,

        /// <summary>
        /// Connector P1, pin 5.
        /// </summary>
        P1Pin5,

        /// <summary>
        /// Connector P1, pin 5.
        /// </summary>
        P1Pin05 = P1Pin5,

        /// <summary>
        /// Connector P1, pin 7.
        /// </summary>
        P1Pin7,

        /// <summary>
        /// Connector P1, pin 7.
        /// </summary>
        P1Pin07 = P1Pin7,

        /// <summary>
        /// Connector P1, pin 8.
        /// </summary>
        P1Pin8,

        /// <summary>
        /// Connector P1, pin 8.
        /// </summary>
        P1Pin08 = P1Pin8,

        /// <summary>
        /// Connector P1, pin 10.
        /// </summary>
        P1Pin10,

        /// <summary>
        /// Connector P1, pin 11.
        /// </summary>
        P1Pin11,

        /// <summary>
        /// Connector P1, pin 12.
        /// </summary>
        P1Pin12,

        /// <summary>
        /// Connector P1, pin 13.
        /// </summary>
        P1Pin13,

        /// <summary>
        /// Connector P1, pin 15.
        /// </summary>
        P1Pin15,

        /// <summary>
        /// Connector P1, pin 16.
        /// </summary>
        P1Pin16,

        /// <summary>
        /// Connector P1, pin 18.
        /// </summary>
        P1Pin18,

        /// <summary>
        /// Connector P1, pin 19.
        /// </summary>
        P1Pin19,

        /// <summary>
        /// Connector P1, pin 21.
        /// </summary>
        P1Pin21,

        /// <summary>
        /// Connector P1, pin 22.
        /// </summary>
        P1Pin22,

        /// <summary>
        /// Connector P1, pin 23.
        /// </summary>
        P1Pin23,

        /// <summary>
        /// Connector P1, pin 24.
        /// </summary>
        P1Pin24,

        /// <summary>
        /// Connector P1, pin 26.
        /// </summary>
        P1Pin26,


        // Pins 27+ exist starting from Model B+

        /// <summary>
        /// Connector P1, pin 27.
        /// </summary>
        P1Pin27,

        /// <summary>
        /// Connector P1, pin 28.
        /// </summary>
        P1Pin28,

        /// <summary>
        /// Connector P1, pin 29.
        /// </summary>
        P1Pin29,

        /// <summary>
        /// Connector P1, pin 31.
        /// </summary>
        P1Pin31,

        /// <summary>
        /// Connector P1, pin 32.
        /// </summary>
        P1Pin32,

        /// <summary>
        /// Connector P1, pin 33.
        /// </summary>
        P1Pin33,

        /// <summary>
        /// Connector P1, pin 35.
        /// </summary>
        P1Pin35,

        /// <summary>
        /// Connector P1, pin 36.
        /// </summary>
        P1Pin36,

        /// <summary>
        /// Connector P1, pin 37.
        /// </summary>
        P1Pin37,

        /// <summary>
        /// Connector P1, pin 38.
        /// </summary>
        P1Pin38,

        /// <summary>
        /// Connector P1, pin 40.
        /// </summary>
        P1Pin40,


        // P5 Connector exist on Rev2 boards (no longer on B+)

        /// <summary>
        /// Connector P5, pin 3.
        /// </summary>
        P5Pin3,

        /// <summary>
        /// Connector P5, pin 3.
        /// </summary>
        P5Pin03 = P5Pin3,

        /// <summary>
        /// Connector P5, pin 4.
        /// </summary>
        P5Pin4,

        /// <summary>
        /// Connector P5, pin 4.
        /// </summary>
        P5Pin04 = P5Pin4,

        /// <summary>
        /// Connector P5, pin 5.
        /// </summary>
        P5Pin5,

        /// <summary>
        /// Connector P5, pin 5.
        /// </summary>
        P5Pin05 = P5Pin5,

        /// <summary>
        /// Connector P5, pin 6.
        /// </summary>
        P5Pin6,

        /// <summary>
        /// Connector P5, pin 6.
        /// </summary>
        P5Pin06 = P5Pin6,

        #endregion

        #region CubieTruck/CubieBoard3 connectors

        // CubieTruck/CubieBoard3 Connector CN8

        /// <summary>
        /// Connector CN8, Pin 5 (PC19)
        /// </summary>
        CB3_CN8Pin5,

        /// <summary>
        /// Connector CN8, Pin 5 (PC19)
        /// </summary>
        CB3_CN8Pin05 = CB3_CN8Pin5,

        /// <summary>
        /// Connector CN8, Pin 6 (PC21)
        /// </summary>
        CB3_CN8Pin6,

        /// <summary>
        /// Connector CN8, Pin 6 (PC21)
        /// </summary>
        CB3_CN8Pin06 = CB3_CN8Pin6,

        /// <summary>
        /// Connector CN8, Pin 7 (PC20)
        /// </summary>
        CB3_CN8Pin7,

        /// <summary>
        /// Connector CN8, Pin 7 (PC20)
        /// </summary>
        CB3_CN8Pin07 = CB3_CN8Pin7,
        
        /// <summary>
        /// Connector CN8, Pin 8 (PC22)
        /// </summary>
        CB3_CN8Pin8,

        /// <summary>
        /// Connector CN8, Pin 8 (PC22)
        /// </summary>
        CB3_CN8Pin08 = CB3_CN8Pin8,

        /// <summary>
        /// Connector CN8, Pin 9 (PB14)
        /// </summary>
        CB3_CN8Pin9,

        /// <summary>
        /// Connector CN8, Pin 9 (PB14)
        /// </summary>
        CB3_CN8Pin09 = CB3_CN8Pin9,

        /// <summary>
        /// Connector CN8, Pin 10 (PB16)
        /// </summary>
        CB3_CN8Pin10,
        
        /// <summary>
        /// Connector CN8, Pin 11 (PB15)
        /// </summary>
        CB3_CN8Pin11,
        
        /// <summary>
        /// Connector CN8, Pin 12 (PB17)
        /// </summary>
        CB3_CN8Pin12,

        /// <summary>
        /// Connector CN8, Pin 15 (PI20)
        /// </summary>
        CB3_CN8Pin15,

        /// <summary>
        /// Connector CN8, Pin 16 (PI14)
        /// </summary>
        CB3_CN8Pin16,
        
        /// <summary>
        /// Connector CN8, Pin 17 (PI21)
        /// </summary>
        CB3_CN8Pin17,
        
        /// <summary>
        /// Connector CN8, Pin 18 (PI15)
        /// </summary>
        CB3_CN8Pin18,
        
        /// <summary>
        /// Connector CN8, Pin 19 (PI3)
        /// </summary>
        CB3_CN8Pin19,
        
        /// <summary>
        /// Connector CN8, Pin 20 (PB3)
        /// </summary>
        CB3_CN8Pin20,
        
        /// <summary>
        /// Connector CN8, Pin 21 (PB2)
        /// </summary>
        CB3_CN8Pin21,
        
        /// <summary>
        /// Connector CN8, Pin 22 (PB4)
        /// </summary>
        CB3_CN8Pin22,
        
        /// <summary>
        /// Connector CN8, Pin 23 (PB18)
        /// </summary>
        CB3_CN8Pin23,
        
        /// <summary>
        /// Connector CN8, Pin 25 (PB19)
        /// </summary>
        CB3_CN8Pin25,

        // CubieTruck/CubieBoard3 Connector CN9

        /// <summary>
        /// Connector CN9, Pin 3 (PG0)
        /// </summary>
        CB3_CN9Pin3,

        /// <summary>
        /// Connector CN9, Pin 3 (PG0)
        /// </summary>
        CB3_CN9Pin03 = CB3_CN9Pin3,

        /// <summary>
        /// Connector CN9, Pin 4 (PG3)
        /// </summary>
        CB3_CN9Pin4,

        /// <summary>
        /// Connector CN9, Pin 4 (PG3)
        /// </summary>
        CB3_CN9Pin04 = CB3_CN9Pin4,

        /// <summary>
        /// Connector CN9, Pin 5 (PG2)
        /// </summary>
        CB3_CN9Pin5,

        /// <summary>
        /// Connector CN9, Pin 5 (PG2)
        /// </summary>
        CB3_CN9Pin05 = CB3_CN9Pin5,

        /// <summary>
        /// Connector CN9, Pin 6 (PG1)
        /// </summary>
        CB3_CN9Pin6,

        /// <summary>
        /// Connector CN9, Pin 6 (PG1)
        /// </summary>
        CB3_CN9Pin06 = CB3_CN9Pin6,

        /// <summary>
        /// Connector CN9, Pin 7 (PG4)
        /// </summary>
        CB3_CN9Pin7,

        /// <summary>
        /// Connector CN9, Pin 7 (PG4)
        /// </summary>
        CB3_CN9Pin07 = CB3_CN9Pin7,

        /// <summary>
        /// Connector CN9, Pin 8 (PG5)
        /// </summary>
        CB3_CN9Pin8,

        /// <summary>
        /// Connector CN9, Pin 8 (PG5)
        /// </summary>
        CB3_CN9Pin08 = CB3_CN9Pin8,
        
        /// <summary>
        /// Connector CN9, Pin 9 (PG6)
        /// </summary>
        CB3_CN9Pin9,

        /// <summary>
        /// Connector CN9, Pin 9 (PG6)
        /// </summary>
        CB3_CN9Pin09 = CB3_CN9Pin9,

        /// <summary>
        /// Connector CN9, Pin 10 (PG7)
        /// </summary>
        CB3_CN9Pin10,
        
        /// <summary>
        /// Connector CN9, Pin 11 (PG8)
        /// </summary>
        CB3_CN9Pin11,
        
        /// <summary>
        /// Connector CN9, Pin 12 (PG9)
        /// </summary>
        CB3_CN9Pin12,     
        
        /// <summary>
        /// Connector CN9, Pin 13 (PG10)
        /// </summary>
        CB3_CN9Pin13,
        
        /// <summary>
        /// Connector CN9, Pin 14 (PG11)
        /// </summary>
        CB3_CN9Pin14

        #endregion
    }
}