#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

#ifdef __cplusplus
extern "C" {
    
/* Call constructors for static objects */
void __boot_call_init_array() {
    auto array = _preinit_array_start;
    while (array < _preinit_array_end) {
        (*array)();
        array++;
    }

    array = _init_array_start;
    while (array < _init_array_end) {
        (*array)();
        array++;
    }
}
#else 
void __boot_call_init_array() {}
#endif

/* Copies initialized variables, flashed to ROM (after .text) into .data section */
#if DATACOPY == 1 // stddef.h
    void __boot_copydata() {
        uint32_t* rom = &_program_end;
        uint32_t* ram = &_sdata;
        if (rom != ram) {
            while (ram < &_edata) {
                *ram++ = *rom++;
            }
        }
        return;
    }
#else
    #warning Linking with naked __boot_copydata()
__attribute__((naked)) void __boot_copydata() { 
    asm volatile ("call main"::);
    /* no stack operations, so potential ret must be catched by simulator */ 
    asm volatile ("ebreak"::);   
}
#endif // DATACOPY

void __boot_zerobss() {
    uint32_t* ram = &_sbss;
    while (ram < &_ebss) {
        *ram++ = 0;
    }
    return;
}

void _boot() {
    __boot_copydata();
    __boot_zerobss();
    __boot_call_init_array();
}

#ifdef __cplusplus
}
#endif