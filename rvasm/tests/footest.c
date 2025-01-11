#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

static uint32_t a = 42;
static uint8_t c = 19;
uint16_t b = 69;

/* Writes 298 [0x12A] into first free addr + margin (&_sstack+1)=(0x10808) */
uint32_t adds() {
    a = a + c;
    b = b + c;
    c = a + b;
    return (a + b + c);
} 

int main() {
    WRITE(FIRST_FREE_ADDR, adds());
    return 0;
}