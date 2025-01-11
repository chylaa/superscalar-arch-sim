#include "../simlib/csr.h"

__attribute__((always_inline)) 
// Inline function to read value from CSR
inline unsigned int read_csr(int csr_num) {
    unsigned int result;
    asm volatile("csrr %0, %1" : "=r"(result) : "I"(csr_num));
    return result; 
}

__attribute__((always_inline)) 
// Inline function to write a value to a CSR
inline void write_csr(int csr_num, unsigned int value) {
    asm volatile ("csrrw x0, %0, %1" :: "i"(csr_num), "r"(value));
}

__attribute__((always_inline)) 
// Inline function to set a bit in a CSR
inline void set_csr_bit(int csr_num, unsigned int bit) {
    asm volatile ("csrs %0, %1" :: "i"(csr_num), "r"(bit));
}

__attribute__((always_inline)) 
// Inline function to clear a bit in a CSR
inline void clear_csr_bit(int csr_num, unsigned int bit) {
    asm volatile ("csrc %0, %1" :: "i"(csr_num), "r"(bit));
}