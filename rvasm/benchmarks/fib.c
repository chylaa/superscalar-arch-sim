
#include "../simlib/stdint.h"
#include "../simlib/stddef.h"

typedef int32_t result_t; 
uint32_t calls;

/* Recursive */
static result_t fib(result_t i) {
    ++calls;
    return (i>1) ? (fib(i-1) + fib(i-2)) : i;
}

/* uint32_t fib(23) = 28657 | 0x6FF1 (92735 calls)  */
/* uint32_t fib(21) = 10946 | 0x2AC2 (35421 calls)  */
int main() {
    calls = 0;
    WRITE_T(FIRST_FREE_ADDR, fib(21));
    WRITE(FIRST_FREE_ADDR+1, calls);
}
