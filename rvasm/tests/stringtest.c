#include "../simlib/stddef.h"
#include "../simlib/stdint.h"
#include "../simlib/strings.h"

string hello = "Hello World\0";

/*
Writes whole "Hello World" string, with NULL terminator, to RAM, starting at FIRST_FREE_ADDR (example addr 0x10800).
[10800:1081F] FFFFFFFF 6C6C6548 6F57206F 00646C72 FFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFF (big-endiann store)
*/
int main() {

    uint8_t i = 0; 
    uint32_t len = (strlen(hello) + 1); 
    
    for (i; i < len; i++) {
        WRITE_BYTE(((uint8_t*)(FIRST_FREE_ADDR) + i), hello[i]);
    }
    return len;
}