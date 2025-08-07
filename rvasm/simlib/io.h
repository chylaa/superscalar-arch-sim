#ifndef IO_H
#define IO_H

#include "stddef.h"
#include "stdint.h"

volatile uint8_t* const IO_CONTROL = (uint8_t* const)0x008F0000;   
volatile const uint8_t* const IO_RX_BUFFER = (const uint8_t* const)0x008F0001;   
volatile uint8_t* const IO_TX_BUFFER = (uint8_t* const)0x008F0002;   

// '0' we fetched character from IO_RX_BUFFER, set to '1' by terminal upon sending us a character
#define IO_CONTROL_RX_BIT 0
// '1' we put character in IO_TX_BUFFER, set to '0' by terminal as ackowledge of receiving
#define IO_CONTROL_TX_BIT 1

#define IO_IS_RX_AVALIABLE() IS_BIT_SET(*IO_CONTROL, IO_CONTROL_RX_BIT)
#define IO_IS_TX_READY() (!IS_BIT_SET(*IO_CONTROL, IO_CONTROL_TX_BIT))

#define IO_SINGAL_TX() SET_BIT(*IO_CONTROL, IO_CONTROL_TX_BIT)
#define IO_SIGNAL_RX() RESET_BIT(*IO_CONTROL, IO_CONTROL_RX_BIT) 

#define IO_ERROR (-1)

void io_init() {
    RESET_BIT(*IO_CONTROL, IO_CONTROL_TX_BIT); // force nothing in TX buffer
    IO_SIGNAL_RX(); // we're ready to receive
}

int io_write(char c) {
    if (IO_IS_TX_READY()) {
        *IO_TX_BUFFER = c;
        IO_SINGAL_TX();
        return c;
    }
    return IO_ERROR;
}

int io_read() {
    if(IO_IS_RX_AVALIABLE()) {
        uint8_t c = *IO_RX_BUFFER;
        IO_SIGNAL_RX();
        return c;
    }
    return IO_ERROR;
}

#endif /* IO_H */