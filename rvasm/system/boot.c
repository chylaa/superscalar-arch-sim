#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

/*
Copies initialized variables, flashed to ROM (after .text) into .data section
If IO_H is defined, automatically calls the io_init() function from io.h
*/
#if DATACOPY == 1 // stddef.h
    void _boot_initdata() {
        uint32_t* rom = &_program_end;
        uint32_t* ram = &_sdata;

        while (ram < &_edata) {
            *ram++ = *rom++;
        }
    #ifdef IO_H
        io_init(); // auto-init if was imported 
    #endif
        return;
    }
#else
    #warning Linking with naked _boot_initdata()
__attribute__((naked)) void _boot_initdata() { 
    asm volatile ("call main"::);
    /* no stack operations, so potential ret must be catched by simulator */ 
    asm volatile ("ebreak"::);   
}
#endif // DATACOPY
