#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

uint32_t DB = 0xdeadbeef;
uint16_t CAFE = 0xcafe;

/* 
    i == 0 
    stores 0xdeadbeef into one of arg registers
*/
uint32_t main()
{
    uint32_t i = 4;
    while (i > 0)
        i-=2;
    
    return (i == 0 ? (uint32_t)DB : (uint32_t)CAFE);
}