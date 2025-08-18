#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

/* Copies initialized variables, flashed to ROM (after .text) into .data section */
#if DATACOPY == 1 // stddef.h
    void _boot_initdata() {
        uint32_t* rom = &_program_end;
        uint32_t* ram = &_sdata;
        if (rom != ram) {
            while (ram < &_edata) {
                *ram++ = *rom++;
            }
        }
        ram = &_sbss;
        while (ram < &_ebss) {
            *ram++ = 0;
        }
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
