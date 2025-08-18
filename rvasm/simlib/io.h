#ifndef IO_H
#define IO_H

#include "stddef.h"
#include "stdint.h"

volatile uint8_t* const IO_RX_BUFFER = (uint8_t* const)0x008F0001;   
volatile uint8_t* const IO_TX_BUFFER = (uint8_t* const)0x008F0002;   

// IO_RX_BUFFER: '0' we fetched character from IO_RX_BUFFER, set to '1' by terminal upon sending us a character
// IO_TX_BUFFER: '1' we put character in IO_TX_BUFFER, set to '0' by terminal as ackowledge of receiving
#define IO_CONTROL_BIT_POSITION 7
/// <summary>Masks the 7 bits of a character in RX/TX buffer.</summary>
#define IO_CHARACTER_MASK 0x7F

#define IO_IS_RX_AVALIABLE() IS_BIT_SET(*IO_RX_BUFFER, IO_CONTROL_BIT_POSITION)
#define IO_IS_TX_READY() (!IS_BIT_SET(*IO_TX_BUFFER, IO_CONTROL_BIT_POSITION))

#define IO_SIGNAL_RX() RESET_BIT(*IO_RX_BUFFER, IO_CONTROL_BIT_POSITION) 

#define IO_ERROR (-1)

void io_init() {
    RESET_BIT(*IO_TX_BUFFER, IO_CONTROL_BIT_POSITION); // force nothing in TX buffer
    IO_SIGNAL_RX(); // we're ready to receive
}

int io_write(char c) {
    if (IO_IS_TX_READY()) {
        uint8_t data = 0;
        // signal TX
        SET_BIT(data, IO_CONTROL_BIT_POSITION);
        // "add" data
        data |= (c & IO_CHARACTER_MASK);
        // update tx buffer
        *IO_TX_BUFFER = data;
        return c;
    }
    return IO_ERROR;
}

int io_read() {
    if(IO_IS_RX_AVALIABLE()) {
        uint8_t c = *IO_RX_BUFFER;
        IO_SIGNAL_RX();
        return c & IO_CHARACTER_MASK;
    }
    return IO_ERROR;
}

#endif /* IO_H */